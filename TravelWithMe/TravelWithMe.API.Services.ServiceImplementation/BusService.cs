using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TravelWithMe.API.Core.Model;
using TravelWithMe.API.Services.ServiceContract;
using TravelWithMe.API.Logging;
using System.ServiceModel.Activation;
using TravelWithMe.API.Services.DataContract;
using TravelWithMe.API.Core.Infra;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.Core.Factories;
using TravelWithMe.Logging.Helper;
using BusInfo = TravelWithMe.API.Services.DataContract.BusInfo;
using BusRate = TravelWithMe.API.Services.DataContract.BusRate;
using BusSchedule = TravelWithMe.API.Services.DataContract.BusSchedule;
using CityPoint = TravelWithMe.API.Services.DataContract.CityPoint;
using Model = TravelWithMe.API.Core.Model;
using SeatArrangement = TravelWithMe.API.Services.DataContract.SeatArrangement;

namespace TravelWithMe.API.Services.ServiceImplementation
{
    [ServerMessageLogger]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class BusService : IBusService
    {
        private const string Source = "BusService";
        #region IBusService members
        public AddBusResponse AddBus(string authId, BusInfo busInfo, string sessionId)
        {
            AddBusResponse response = new AddBusResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, null, authId, out busOperatorId, sessionId))
                {
                    try
                    {
                        IBusProvider busProvider = BusProviderFactory.GetBusProvider();
                        ICacheProvider cacheProvider = CacheProviderFactory.GetCacheProvider();
                        Core.Model.BusInfo bus = busInfo.ToDataModel();
                        bus.BusOperatorId = busOperatorId;
                        bus.FromLoc.Id = cacheProvider.GetCityId(bus.FromLoc.Name);
                        bus.ToLoc.Id = cacheProvider.GetCityId(bus.ToLoc.Name);
                        bus.BusTripId = busProvider.AddBus(bus);
                        if (bus.BusTripId > 0)
                        {
                            response.IsSuccess = true;
                            response.BusTripId = bus.BusTripId;
                            
                            ConcurrentDictionary<int, Core.Model.BusInfo> busCache = cacheProvider.GetBusCache();
                            busCache[bus.BusTripId] = bus;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Please try again.";
                        }
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Something is not quite right here. Please try again later.";
                        Logger.LogException(e, Source, "AddBus", Severity.Normal);
                    }
                }
            }
            return response;
        }

        public SetBusStatusResponse SetBusStatus(string authId, string bustripid, string sessionId, string isEnabled)
        {
            SetBusStatusResponse response = new SetBusStatusResponse() {IsSuccess = true};
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, bustripid, authId, out busOperatorId, sessionId))
                {
                    try
                    {
                        ICacheProvider cacheProvider = CacheProviderFactory.GetCacheProvider();
                        Model.BusInfo cachedBus = cacheProvider.GetBusCache()[ParseBusTripId(bustripid)];
                        IBusProvider busProvider = BusProviderFactory.GetBusProvider();
                        if(busProvider.SetBusStatus(cachedBus.BusTripId, bool.Parse(isEnabled)))
                        {
                            cachedBus.IsEnabled = bool.Parse(isEnabled);
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Unable to update the bus status!!";
                        }
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Something is not quite right here. Please try again later.";
                        Logger.LogException(e, Source, "SetBusStatus", Severity.Normal);
                    }
                }
            }
            return response;
        }

        public SetBusStatusResponse SetBusPublishStatus(string authId, string bustripid, string sessionId, string isPublished)
        {
            SetBusStatusResponse response = new SetBusStatusResponse() { IsSuccess = true };
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string accontId = string.Empty;
                if (ValidateAccount(response, bustripid, authId, out accontId, sessionId))
                {
                    try
                    {
                        bool isPub = bool.Parse(isPublished);
                        if (!isPub && !SessionProviderFactory.GetSessionProvider().IsAdministrator(sessionId, accontId))
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Sorry! You are not allowed to un publish this bus!!";
                            return response;
                        }
                        ICacheProvider cacheProvider = CacheProviderFactory.GetCacheProvider();
                        Model.BusInfo cachedBus = cacheProvider.GetBusCache()[ParseBusTripId(bustripid)];
                        IBusProvider busProvider = BusProviderFactory.GetBusProvider();
                        if (busProvider.SetBusPublishStatus(cachedBus.BusTripId, isPub))
                        {
                            cachedBus.IsPublished = isPub;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Unble to update the bus publish status!!";
                        }
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Something is not quite right here. Please try again later.";
                        Logger.LogException(e, Source, "SetBusStatus", Severity.Normal);
                    }
                }
            }
            return response;
        }

        public DeleteBusResponse DeleteBus(string authId, string bustripid, string sessionId)
        {
            DeleteBusResponse response = new DeleteBusResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, bustripid, authId, out busOperatorId, sessionId))
                {
                    try
                    {
                        IBusProvider busProvider = BusProviderFactory.GetBusProvider();
                        int id;
                        bool isParsed = int.TryParse(bustripid, out id);
                        if (!isParsed)
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Invalid identifier.";
                            return response;
                        }
                        bool delStatus = busProvider.DeleteBus(id);
                        if (delStatus)
                        {
                            response.IsSuccess = true;
                            ICacheProvider cacheProvider = CacheProviderFactory.GetCacheProvider();
                            ConcurrentDictionary<int, Core.Model.BusInfo> busCache = cacheProvider.GetBusCache();
                            Core.Model.BusInfo busInfo = null;
                            busCache.TryRemove(id, out busInfo);
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Please try again.";
                        }
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Something is not quite right here. Please try again later.";
                        Logger.LogException(e, Source, "DeleteBus", Severity.Normal);
                    }

                }
            }
            return response;
        }

        public GetAllBusesResponse GetAllBuses(string authId, string sessionId)
        {
            GetAllBusesResponse response = new GetAllBusesResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, null, authId, out busOperatorId, sessionId))
                {
                    try
                    {
                        ICacheProvider provider = CacheProviderFactory.GetCacheProvider();
                        List<Core.Model.BusInfo> buses =
                            provider.GetBusCache().Values.Where(bus => string.Equals(busOperatorId, bus.BusOperatorId)).
                                ToList();
                        if (buses != null && buses.Count > 0)
                        {
                            response.Buses = buses.ToDataContract();
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "No buses found! If you have added any buses then please report!";
                        }
                    }
                    catch (Exception exception)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Something is not quite right here. Please try again later.";
                        Logger.LogException(exception, Source, "GetAllBuses", Severity.Normal);
                    }
                }
            }
            return response;
        }

        public UpdateBusDetailsResponse UpdateBusDetails(string authId, string bustripId, BusInfo busInfo, string sessionId)
        {
            busInfo.BusTripId = ParseBusTripId(bustripId);
            UpdateBusDetailsResponse response = new UpdateBusDetailsResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, bustripId, authId, out busOperatorId, sessionId))
                {
                    try
                    {
                        IBusProvider busProvider = BusProviderFactory.GetBusProvider();
                        Model.BusInfo busInfoModel = busInfo.ToDataModel();
                        ICacheProvider cache = CacheProviderFactory.GetCacheProvider();
                        Model.BusInfo cachedBus = cache.GetBusCache()[busInfoModel.BusTripId];
                        busInfoModel.FromLoc.Id = cache.GetCityId(busInfoModel.FromLoc.Name);
                        busInfoModel.ToLoc.Id = cache.GetCityId(busInfoModel.ToLoc.Name);
                        if (!cachedBus.IsPublished)
                        {
                            bool status = busProvider.UpdateBusDetails(busInfoModel);
                            if (status)
                            {
                                response.IsSuccess = true;
                                cachedBus.BusName = busInfoModel.BusName;
                                cachedBus.ArrivalTime = busInfoModel.ArrivalTime;
                                cachedBus.DepartureTime = busInfoModel.DepartureTime;
                                cachedBus.FromLoc = busInfoModel.FromLoc;
                                cachedBus.ToLoc = busInfoModel.ToLoc;
                                cachedBus.IsAC = busInfoModel.IsAC;
                                cachedBus.BusType = busInfoModel.BusType;
                            }
                            else
                            {
                                response.IsSuccess = false;
                                response.ErrorMessage = "Bus details not updated. Please try again.";
                            }
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Sorry! You cannot update details of published bus!";
                        }
                    }
                    catch (Exception exception)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Something is not quite right here. Please try again later.";
                        Logger.LogException(exception, Source, "UpdateBusDetails", Severity.Normal);
                    }
                }
            }
            return response;
        }

        public GetBusScheduleResponse GetBusScheduleDetails(string authId, string busTripId ,string sessionId)
        {
            GetBusScheduleResponse response = new GetBusScheduleResponse() { IsSuccess = true };
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, busTripId, authId, out busOperatorId, sessionId))
                {
                    try
                    {
                        ICacheProvider cacheProvider = CacheProviderFactory.GetCacheProvider();
                        Model.BusInfo cachedBus = cacheProvider.GetBusCache()[ParseBusTripId(busTripId)];
                        response.BusSchedule = cachedBus.BusSchedule.ToDataContract();
                        return response;
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Something is not quite right here. Please try again later.";
                        Logger.LogException(e, Source, "GetBusScheduleResponse", Severity.Normal);
                    }
                }
            }
            return response;
        }

        public UpdateBusScheduleResponse UpdateBusScheduleDetails(string authId, string busTripId, BusSchedule busSchedule, string sessionId)
        {
            UpdateBusScheduleResponse response = new UpdateBusScheduleResponse() { IsSuccess = true };
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, busTripId, authId, out busOperatorId, sessionId))
                {
                    try
                    {
                        int busId = ParseBusTripId(busTripId);
                        ICacheProvider cacheProvider = CacheProviderFactory.GetCacheProvider();
                        Model.BusInfo cachedBus = cacheProvider.GetBusCache()[busId];
                        if (!cachedBus.IsPublished)
                        {
                            Model.BusSchedule busScheduleModel = busSchedule.ToDataModel();
                            IBusProvider busProvider = BusProviderFactory.GetBusProvider();
                            bool success = busProvider.UpdateBusScheduleDetails(busId, busScheduleModel);
                            if (success)
                            {
                                cachedBus.BusSchedule = busScheduleModel;
                            }
                            else
                            {
                                response.IsSuccess = false;
                                response.ErrorMessage = "Frequency Details not updated. Please try again.";
                            }
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Schedule change is not allowed on published bus";
                        }
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Something is not quite right here. Please try again later.";
                        Logger.LogException(e, Source, "UpdateBusScheduleDetails", Severity.Normal);
                    }
                }
            }
            return response;
        }

        public GetBusResponse GetBus(string authId, string bustripId, string sessionId)
        {
            GetBusResponse response = new GetBusResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, bustripId, authId, out busOperatorId, sessionId))
                {
                    try
                    {
                        int id = Convert.ToInt32(bustripId);
                        IBusProvider busProvider = BusProviderFactory.GetBusProvider();
                        response.BusInfo = busProvider.GetBus(id).ToDataContract();
                        if (response.BusInfo != null)
                            response.IsSuccess = true;
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Please try again.";
                        }
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Something is not quite right here. Please try again later.";
                        Logger.LogException(e, Source, "GetBus", Severity.Normal);
                    }
                }
            }
            return response;
        }

        public GetDefaultSeatMapsResponse GetDefaultSeatMaps(string authId, string sessionId)
        {
            GetDefaultSeatMapsResponse response = new GetDefaultSeatMapsResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, null, authId, out busOperatorId, sessionId))
                {
                    try
                    {
                        ICacheProvider provider = CacheProviderFactory.GetCacheProvider();
                        response.SeatMaps =
                            provider.GetSeatMapCache().Select(keyValue => new SeatArrangement() {Id = keyValue.Key}).
                                ToList();
                        response.IsSuccess = true;
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = e.Message;
                        Logger.LogException(e, Source, "GetDefaultSeatMaps", Severity.Normal);
                    }
                }
            }
            return response;
        }

        public GetSeatMapResponse GetSeatMap(string authId, string sessionId, string seatMapId)
        {
            GetSeatMapResponse response = new GetSeatMapResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, null, authId, out busOperatorId, sessionId))
                {
                    try
                    {
                        ICacheProvider provider = CacheProviderFactory.GetCacheProvider();
                        var seatMapCache = provider.GetSeatMapCache();
                        response.SeatMap = seatMapCache[int.Parse(seatMapId)];
                        response.IsSuccess = true;
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = e.Message;
                        Logger.LogException(e, Source, "GetSeatMap", Severity.Normal);
                    }
                }
            }
            return response;
        }

        public GetSeatMapResponse GetBusSeatMap(string authId, string busTripId, string sessionId)
        {
            GetSeatMapResponse response = new GetSeatMapResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, busTripId, authId, out busOperatorId, sessionId))
                {
                    try
                    {
                        ICacheProvider provider = CacheProviderFactory.GetCacheProvider();
                        Core.Model.BusInfo bus = provider.GetBusCache()[int.Parse(busTripId)];
                        var seatMapCache = provider.GetSeatMapCache();
                        response.SeatMap = seatMapCache.ContainsKey(bus.SeatMapId)
                                               ? seatMapCache[bus.SeatMapId]
                                               : string.Empty;
                        response.IsSuccess = true;
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = e.Message;
                        Logger.LogException(e, Source, "GetBusSeatMap", Severity.Normal);
                    }
                }
            }
            return response;
        }

        public UpdateSeatMapResponse UpdateSeatMap(string authId, UpdateSeatMapRQ seatMapRQ, string sessionId)
        {
            UpdateSeatMapResponse response = new UpdateSeatMapResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, seatMapRQ.BusTripId.ToString(), authId, out busOperatorId, sessionId))
                {
                    try
                    {
                        IBusProvider busProvider = BusProviderFactory.GetBusProvider();
                        ICacheProvider cacheProvider = CacheProviderFactory.GetCacheProvider();
                        var cachedBus = cacheProvider.GetBusCache()[seatMapRQ.BusTripId];
                        if (!cachedBus.IsPublished)
                        {
                            int seatMapId = seatMapRQ.SeatMapId;
                            if(seatMapId == 0)
                            {
                                seatMapId = busProvider.AddSeatmap(seatMapRQ.SeatMap);
                            }
                            busProvider.UpdateSeatMap(seatMapRQ.BusTripId, seatMapId);
                            cachedBus.SeatMapId = seatMapId;
                            var seatMapCache = cacheProvider.GetSeatMapCache();
                            seatMapCache[seatMapId] = seatMapRQ.SeatMap;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Changes in the seatmap of published bus is not allowed!";
                        }
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Something is not quite right here. Please try again later.";
                        Logger.LogException(e, Source, "UpdateSeatMap", Severity.Normal);
                    }
                }
            }
            return response;
        }

        public UpdateCityPointResponse UpdateCityPoint(string authId, string bustripId, CityPoint cityPoint, string sessionId)
        {
            UpdateCityPointResponse response = new UpdateCityPointResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, bustripId, authId, out busOperatorId, sessionId))
                {
                    try
                    {
                        IBusProvider busProvider = BusProviderFactory.GetBusProvider();
                        ICacheProvider cacheProvider = CacheProviderFactory.GetCacheProvider();
                        var cpModel = cityPoint.ToDataModel();
                        cpModel.CityId = cacheProvider.GetCityId(cpModel.CityName);
                        cpModel.CPId = cacheProvider.GetCityPointId(cpModel);
                        var cachedBus = cacheProvider.GetBusCache()[ParseBusTripId(bustripId)];
                        bool success = busProvider.SaveBusCityPoint(cachedBus.BusTripId, cpModel);
                        if (success)
                        {
                            response.IsSuccess = true;
                            response.CPId = cpModel.CPId;
                            cachedBus.CityPoints = cachedBus.CityPoints ?? new List<Model.CityPoint>();
                            bool isNew = true;
                            for (int i = 0; i < cachedBus.CityPoints.Count;i++)
                            {
                                if(cachedBus.CityPoints[i].CPId == cityPoint.CPId)
                                {
                                    isNew = false;
                                    cachedBus.CityPoints[i] = cpModel;
                                }
                            }
                            if (isNew)
                            {
                                cachedBus.CityPoints.Add(cpModel);
                            }
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "CityPoint not updated. Please try again.";
                        }
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Something is not quite right here. Please try again later.";
                        Logger.LogException(e, Source, "UpdateCityPoint", Severity.Normal);
                    }
                }
            }
            return response;
        }

        public DeleteCityPointResponse DeleteCityPoint(string authId, string bustripId, string cpId, string sessionId)
        {
            DeleteCityPointResponse response = new DeleteCityPointResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, bustripId, authId, out busOperatorId, sessionId))
                {
                    try
                    {
                        ICacheProvider cacheProvider = CacheProviderFactory.GetCacheProvider();
                        IBusProvider busProvider = BusProviderFactory.GetBusProvider();
                        var cachedBus = cacheProvider.GetBusCache()[ParseBusTripId(bustripId)];
                        bool success = busProvider.DeleteBusCityPoint(cachedBus.BusTripId, Convert.ToInt32(cpId));
                        if (success)
                        {
                            response.IsSuccess = true;
                            cachedBus.CityPoints.RemoveAll(cp => cp.CPId == Convert.ToInt32(cpId));
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "CityPoint not deleted. Please try again.";
                        }
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Something is not quite right here. Please try again later.";
                        Logger.LogException(e, Source, "DeleteCityPoint", Severity.Normal);
                    }
                }
            }
            return response;
        }

        public GetAllCityPointResponse GetAllCityPoints(string authId, string bustripId, string sessionId)
        {
            GetAllCityPointResponse response = new GetAllCityPointResponse() {IsSuccess = true};
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, bustripId, authId, out busOperatorId, sessionId))
                {
                    try
                    {
                        ICacheProvider cacheProvider = CacheProviderFactory.GetCacheProvider();
                        Model.BusInfo cachedBus = cacheProvider.GetBusCache()[ParseBusTripId(bustripId)];
                        response.CityPoints = cachedBus.CityPoints.ToDataContract();
                        return response;
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Something is not quite right here. Please try again later.";
                        Logger.LogException(e, Source, "AllCityPoints", Severity.Normal);
                    }
                }
            }
            return response;
        }

        public AddBusRateResponse AddBusRate(string authId, string bustripId, BusRate busRate, string sessionId)
        {
            AddBusRateResponse response = new AddBusRateResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, bustripId, authId, out busOperatorId, sessionId))
                {
                    try
                    {
                        IBusProvider busProvider = BusProviderFactory.GetBusProvider();
                        int rateId = busProvider.AddBusRate(ParseBusTripId(bustripId), busRate.ToDataModel());
                        if (rateId > 0)
                        {
                            response.IsSuccess = true;
                            response.RateId = rateId;
                            busRate.RateId = rateId;
                            ICacheProvider cacheProvider = CacheProviderFactory.GetCacheProvider();
                            Model.BusInfo cachedBus = cacheProvider.GetBusCache()[ParseBusTripId(bustripId)];
                            if(cachedBus !=null)
                            {
                                cachedBus.BusRates = cachedBus.BusRates ?? new List<Model.BusRate>();
                                cachedBus.BusRates.Add(busRate.ToDataModel());
                            }
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "BusRate not added. Please try again.";
                        }
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Something is not quite right here. Please try again later.";
                        Logger.LogException(e, Source, "AddBusRate", Severity.Normal);
                    }
                }
            }
            return response;
        }

        public DeleteBusRateResponse DeleteBusRate(string authId, string bustripId, string busRateId, string sessionId)
        {
            DeleteBusRateResponse response = new DeleteBusRateResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, bustripId, authId, out busOperatorId, sessionId))
                {
                    try
                    {
                        int rateid = Convert.ToInt32(busRateId);
                        IBusProvider busProvider = BusProviderFactory.GetBusProvider();
                        bool success = busProvider.DeleteBusRate(ParseBusTripId(bustripId), rateid);
                        if (success)
                        {
                            response.IsSuccess = true;

                            ICacheProvider cacheProvider = CacheProviderFactory.GetCacheProvider();
                            Model.BusInfo cachedBus = cacheProvider.GetBusCache()[ParseBusTripId(bustripId)];
                            if (cachedBus != null && cachedBus.BusRates!=null)
                            {
                                cachedBus.BusRates.RemoveAll(x => x.RateId == rateid);
                            }
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "BusRate not deleted. Please try again.";
                        }
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Something is not quite right here. Please try again later.";
                        Logger.LogException(e, Source, "DeleteBusRate", Severity.Normal);
                    }
                }
            }
            return response;
        }

        public UpdateBusRateResponse UpdateBusRate(string authId, string bustripId, BusRate busRate, string sessionId)
        {
            UpdateBusRateResponse response = new UpdateBusRateResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, bustripId, authId, out busOperatorId, sessionId))
                {
                    try
                    {
                        IBusProvider busProvider = BusProviderFactory.GetBusProvider();
                        bool success = busProvider.UpdateBusRate(ParseBusTripId(bustripId), busRate.ToDataModel());
                        if (success)
                        {
                            response.IsSuccess = true;

                            ICacheProvider cacheProvider = CacheProviderFactory.GetCacheProvider();
                            Model.BusInfo cachedBus = cacheProvider.GetBusCache()[ParseBusTripId(bustripId)];
                            if (cachedBus != null && cachedBus.BusRates !=null && cachedBus.BusRates.Count>0)
                            {
                                cachedBus.BusRates.RemoveAll(x => x.RateId == busRate.RateId);
                                cachedBus.BusRates.Add(busRate.ToDataModel());
                            }
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "BusRate not updated. Please try again.";
                        }
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Something is not quite right here. Please try again later.";
                        Logger.LogException(e, Source, "UpdateBusRate", Severity.Normal);
                    }
                }
            }
            return response;
        }

        public GetAllBusRatesResponse GetAllBusRates(string authId, string bustripId, string sessionId)
        {
            GetAllBusRatesResponse response = new GetAllBusRatesResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, bustripId, authId, out busOperatorId, sessionId))
                {
                    try
                    {
                        ICacheProvider cacheProvider = CacheProviderFactory.GetCacheProvider();
                        List<Core.Model.BusRate> rates = cacheProvider.GetBusCache()[ParseBusTripId(bustripId)].BusRates;

                        if (rates != null)
                        {
                            rates.Sort((r1, r2) => DateTime.Compare(r1.DateFrom, r2.DateFrom));
                            response.Rates = rates.ToDataContract();
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Please try again.";
                        }
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Something is not quite right here. Please try again later.";
                        Logger.LogException(e, Source, "GetAllBusRates", Severity.Normal);
                    }
                }
            }
            return response;
        }
        #endregion

        #region helper methods

        private int ParseBusTripId(string busTripId)
        {
            int id = 0;
            bool isSuccess = int.TryParse(busTripId, out id);
            return isSuccess ? id : -1;
        }

        private bool ValidateAccount(BaseResponse response, string busTripId, string authId, out string accountID, string sessionId)
        {
            IAuthenticationProvider authProvider = AuthenticationProviderFactory.GetAuthenticationProvider();
            if (!authProvider.Validate(authId))
            {
                response.IsSuccess = false;
                response.ErrorMessage = "Validation failed! Not a valid user, Please login and try again!!";
                accountID = null;
                return false;
            }
            accountID = authProvider.GetAccountId(authId);
            if (string.IsNullOrEmpty(accountID))
            {
                response.IsSuccess = false;
                response.ErrorMessage = "Validation failed! User not found, Please loging and try again!!";
                return false;
            }
            ISessionProvider sessionProvider = SessionProviderFactory.GetSessionProvider();
            bool isMatch = sessionProvider.IsBusOperator(sessionId, accountID);
            if (!isMatch)
            {
                response.IsSuccess = false;
                response.ErrorMessage = "Validation failed! your are not a bus operator!!";
                return false;
            }
            if (!string.IsNullOrEmpty(busTripId))
            {
                Model.BusInfo cachedBus =
                    CacheProviderFactory.GetCacheProvider().GetBusCache()[ParseBusTripId(busTripId)];
                if (!string.Equals(cachedBus.BusOperatorId, accountID, StringComparison.OrdinalIgnoreCase))
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = "Validation failed! Invalid bus!";
                }
            }
            return true;
        }
        #endregion
    }
}
