var MockDataProvider = function () {
    this.SeatMap = function () {
        this.Name = '';
        this.BusType = '';
        this.SeatCount = 0;
        this.BerthCount = 0;
        this.IsSelected = false;
        this.Decks = [];
    };

    this.Deck = function () {
        this.DeckType = '';
        this.Size = '';
        this.Seats = [];
    };

    this.SeatData = function () {
        this.SeatType = '';
        this.Row = -1;
        this.Col = -1;
        this.Label = '';
        this.SeatNumber = '';
        this.Price = 0;
    };

    this.GetBusList = function () {
        var fromcityList = [{ Name: 'Latur', Code: 'LUR' }, { Name: 'Pune', Code: 'PUN' }, { Name: 'Delhi', Code: 'DEL'}];
        var tocityList = [{ Name: 'Mumbai', Code: 'MUM' }, { Name: 'Nashik', Code: 'NAS' }, { Name: 'Udagir', Code: 'UDA'}];
        var amenities = ['Television', 'Charging Point', 'Blanket', 'Water Bottle'];
        var busTypes = ['Sleeper', 'Semi Sleeper', 'Seating'];
        var acnonac = [true, false, true, false, true, true, false];
        var busFrequencies = [
            {
                Frequency: 'Daily',
                Weekdays: null,
                DateRanges: null
            },
            {
                Frequency: 'SpecificWeekDays',
                Weekdays: ['Sun', 'Tue', 'Wed'],
                DateRanges: null
            },
            {
                Frequency: 'SpecificDates',
                Weekdays: null,
                DateRanges: [
                    { RangeId: 1, From: 'Jan 12, 2012', To: 'Jan 12, 2012' },
                    { RangeId: 2, From: 'Feb 13, 2012', To: 'Feb 13, 2012' },
                    { RangeId: 3, From: 'Dec 30, 2012', To: 'Dec 30, 2012'}]
            }
        ];
        var busSizes = ['2x1', '2x2'];
        var berths = [1, 2];
        var seats = [34, 48, 38, 60];

        var busList = {
            Buses: []
        };

        for (var i = 1; i < 20; i++) {
            var from = Rand(fromcityList);
            var to = Rand(tocityList);
            var bus = {
                BusTripId: i.toString(),
                BusOperatorId: i,
                BusName: 'Bus' + i,
                FromLoc: { CityName: from.Name, CityCode: from.Code },
                ToLoc: { CityName: to.Name, CityCode: to.Code },
                DepartureTime: '10:30AM',
                ArrivalTime: '07:00PM',
                BusType: Rand(busTypes),
                IsAC: Rand(acnonac),
                BusSchedule: Rand(busFrequencies),
                CityPoints: [
                    { CPId: 1, CPTime: '10:30AM', CPName: 'Ganjgolai', IsDropOffPoint: false, IsPickupPoint: true, CityName: from.Name, CityCode: from.Code },
                    { CPId: 2, CPTime: '11:30AM', CPName: 'ShiwajiChawk', IsDropOffPoint: false, IsPickupPoint: true, CityName: from.Name, CityCode: from.Code },
                    { CPId: 3, CPTime: '12:30AM', CPName: '5Number', IsDropOffPoint: false, IsPickupPoint: true, CityName: from.Name, CityCode: from.Code },
                    { CPId: 4, CPTime: '04:00PM', CPName: 'Hadapsar', IsDropOffPoint: true, IsPickupPoint: false, CityName: to.Name, CityCode: to.Code },
                    { CPId: 5, CPTime: '05:00PM', CPName: 'Shiwaji Nagar', IsDropOffPoint: true, IsPickupPoint: false, CityName: to.Name, CityCode: to.Code },
                    { CPId: 6, CPTime: '07:00PM', CPName: 'Pimpari', IsDropOffPoint: true, IsPickupPoint: false, CityName: to.Name, CityCode: to.Code }
                ],
                BusRates: [
                    { RateId: 1, DateFrom: 'Feb 1, 2012', DateTo: 'Aug 1, 2012', WeekDayRate: 250, WeekEndRate: 300 },
                    { RateId: 2, DateFrom: 'Aug 2, 2012', DateTo: 'Dec 1, 2012', WeekDayRate: 350, WeekEndRate: 400 }
                ],
                Amenities: RandArr(amenities),
                SeatMap: Rand(new MockDataProvider().GetDefaultSeatMaps())
            };
            busList.Buses.push(bus);
        }
        busList.Buses[0].ArrivalTime = busList.Buses[0].ArrivalTime + '(next day)';
        busList.Buses[1].ArrivalTime = busList.Buses[1].ArrivalTime + '(day-3)';
        return busList;
    };

    this.GetDefaultSeatMaps = function () {
        var SeatMaps = [];
        var map = new this.SeatMap();
        map.Name = "Volvo sleeper";
        map.BusType = 'Sleeper';
        map.SeatCount = 9;
        map.BerthCount = 36;
        var lowerDeck = this.GetDeck('lowerDeck', 9, 10, 3, 0);
        map.Decks.push(lowerDeck);
        var upperDeck = this.GetDeck('upperDeck', 0, 26, 3, lowerDeck.LastAddedSeatNumber);
        map.Decks.push(upperDeck);
        SeatMaps.push(map);

        map = new this.SeatMap();
        map.Name = "Volvo Seater";
        map.BusType = 'Seater';
        map.SeatCount = 40;
        map.BerthCount = 0;
        lowerDeck = this.GetDeck('lowerDeck', 40, 0, 4, 0);
        map.Decks.push(lowerDeck);
        SeatMaps.push(map);

        map = new this.SeatMap();
        map.IsSelected = true;
        map.Name = "Sleeper";
        map.BusType = 'Sleeper';
        map.SeatCount = 0;
        map.BerthCount = 36;
        lowerDeck = this.GetDeck('lowerDeck', 0, 18, 3, 0);
        map.Decks.push(lowerDeck);
        upperDeck = this.GetDeck('upperDeck', 0, 18, 3, lowerDeck.LastAddedSeatNumber);
        map.Decks.push(upperDeck);
        SeatMaps.push(map);
        return SeatMaps;
    };

    this.GetDeck = function (deckType, seatCnt, berthCnt, rows, lastSeatAdded) {
        var cols = parseInt((seatCnt + berthCnt) / rows);
        var remider = (seatCnt + berthCnt) % rows;
        var addedSeatCnt = lastSeatAdded ? lastSeatAdded : 0;
        var deck = new this.Deck();
        deck.DeckType = deckType;
        deck.Size = rows == 3 ? '2X1' : '2X2';
        for (var col = 0; col < cols; col++) {
            for (var row = 0; row < rows; row++) {
                var seat = new this.SeatData();
                seat.SeatType = addedSeatCnt >= seatCnt ? 'availableBerthH' : 'availableSeat';
                seat.SeatNumber = addedSeatCnt + 1;
                seat.Row = rows == 3 && row == 2 ? row + 1 : row;
                seat.Col = col;
                seat.Label = (seat.SeatType == 'availableSeat' ? 'S' : 'B') + seat.SeatNumber;
                seat.Price = 100 * (cols - col);
                deck.Seats.push(seat);
                addedSeatCnt++;
            }
        }
        for (var i = 0; i < remider; i++) {
            seat = new this.SeatData();
            seat.SeatType = berthCnt != 0 ? 'availableBerthH' : 'availableSeat';
            seat.SeatNumber = addedSeatCnt + 1;
            seat.Row = rows == 3 && i == 2 ? i + 1 : i;
            seat.Col = cols;
            seat.Label = (seat.SeatType == 'availableSeat' ? 'S' : 'B') + seat.SeatNumber;
            seat.Price = 100;
            deck.Seats.push(seat);
            addedSeatCnt++;
        }
        deck.LastAddedSeatNumber = addedSeatCnt;
        return deck;
    };

    this.GetCityList = function (request, response) {
        var cityList = ['Latur-(LUR)', 'Pune-(PUN)', 'Pimpari-(PIM)', 'Chinchwad-(CHD)', 'Mumbai-(MUM)', 'Nashik-(NAS)', 'Aurangabad-(AUR)', 'Bengalore-(BLR)', 'Udagir-(UDA)', 'Nanded-(NAN)'];
        var filteredList = [];
        for (var city in cityList)           //Go through every item in the array
        {
            var matches = false;     //Does this meet our criterium?
            matches = new RegExp(request.term, 'i').test(cityList[city]);
            if (matches) {
                filteredList.push(cityList[city]);
            }
        }
        response(filteredList);
    };

    this.GetCityPointList = function (request, response) {
        var cityPoints = ['Ganjgolai', 'ShiwajiChawk', '5Number', 'Hadapsar', 'Shiwaji Nagar', 'Pimpari'];
        response(cityPoints);
    };

    var Rand = function (array) {
        var randNum = Math.floor((Math.random() * array.length) + 1) - 1;
        return array[randNum];
    };

    var RandArr = function (array, cnt) {
        var retArr = [];
        for (var i = 0; i < cnt; i++) {
            var randNum = Math.floor((Math.random() * array.length) + 1) - 1;
            retArr.push(array[randNum]);
        }
        return retArr;
    };

    this.GetSearchResults = function () {
        var results = [];
        for (var i = 0; i < 10; i++) {
            var itin = {
                OId: i,
                BTId: i
            };
            results.push(itin);
        }
    };

    this.GetRecentSearches = function () {
        var fromcityList = [{ Name: 'Latur', Code: 'LUR' }, { Name: 'Pune', Code: 'PUN' }, { Name: 'Delhi', Code: 'DEL'}];
        var tocityList = [{ Name: 'Mumbai', Code: 'MUM' }, { Name: 'Nashik', Code: 'NAS' }, { Name: 'Udagir', Code: 'UDA'}];
        var searches = [];
        for (var i = 0; i < 5; i++) {
            var search = {
                Id: i,
                From: Rand(fromcityList),
                To: Rand(tocityList)
            };
            searches.push(search);
        }
        return searches;
    };
};

