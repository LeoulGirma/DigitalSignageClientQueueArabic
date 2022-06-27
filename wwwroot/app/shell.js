//import { setTimeout } from "timers";

define(['plugins/router', 'knockout', 'durandal/app','jquery','signalr', 'ticker-easing','ticker-easy','ticker'], function (router, ko, app, $,signalr) {

    ko.bindingHandlers.fadeVisible = {
        init: function (element, valueAccessor) {
            // Initially set the element to be instantly visible/hidden depending on the value
            var value = valueAccessor();
            $(element).toggle(ko.utils.unwrapObservable(value)); // Use "unwrapObservable" so we can handle values that may or may not be observable
        },
        update: function (element, valueAccessor) {
            // Whenever the value subsequently changes, slowly fade the element in or out
            var value = valueAccessor();
            ko.utils.unwrapObservable(value) ? $(element).fadeIn() : $(element).fadeOut();
        }
    };

    function CurrencyModel() {
        this.Model = ko.observable({
            Currency: ko.observable("-"),
            Abbreviation: ko.observable("-"),
            Difference: ko.observable("0.00"),
            Flag: ko.observable(""),
            BranchInAmharic: ko.observable(""),
            Buying: ko.observable("0.00"),
            Selling: ko.observable("0.00"),
            UpdateDate: ko.observable(""),
        });
    }

    function RSSNews() {
        this.Model = ko.observable({
            Title: ko.observable(""),
            Content: ko.observable("")
        });
    }
    var vm = {};
    vm.Months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'June', 'July', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    vm.FullMonths = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
    vm.Dates = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
    vm.FormatCurrencyDate = function (value) {
        if (value == null || value == undefined) {
            return "";
        }
        var date = new Date(value);
        var output = vm.Months[date.getMonth()] + ' ' + date.getDate() + ', ' + date.getFullYear();
        return output;
    }

    vm.FormatDate = function (value) {
        if (value == null || value == undefined) {
            return "";
        }
        var date = value;
        var output = date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();
        return output;
    }
    vm.GetYear = function (value) {
        if (value == null || value == undefined) {
            return "";
        }
        var date = new Date(value);
        var output = date.getFullYear();
        return output;
    }

    //Count Date
    vm.countDate = ko.observable(new Date('Jan 1, 2021 00:00:00').getTime());
    vm.countDateToday = ko.observable(new Date().getTime());
    vm.d = ko.observable("");
    vm.h = ko.observable("");
    vm.m = ko.observable("");
    vm.s = ko.observable("");

    vm.getCountDate = function () {
        vm.countDateToday(new Date());
        vm.countDate(new Date(vm.countDate()));
        var gap = vm.countDate() - vm.countDateToday();
        var second = 1000;
        var minute = second * 60;
        var hour = minute * 60;
        var day = hour * 24;

        vm.d(Math.floor(gap / (day)));
        vm.h(Math.floor((gap % (day)) /(hour)));
        vm.m(Math.floor((gap % (hour)) /(minute)));
        vm.s(Math.floor((gap % (minute)) /(second)));
    }




    //Style
    
    vm.router = router;

    vm.IsLiveStream = ko.observable(false);
    vm.LiveStreamVideo = ko.observable("");
    vm.RSST = ko.observable(1);
    vm.RSSText = ko.observable("");
    vm.ChangeRSS = ko.observable(true);
    vm.RSSLength = ko.observable(0);
    vm.RSSNews = ko.observable("");
    vm.RSSStyle = ko.observable("");
    vm.RSSCategory = ko.observable("");
    vm.RSSInterval = ko.observable();
    vm.RSSDate = ko.observable("");
    vm.RSSTitle = ko.observable("");
    vm.WeatherLogoPath = ko.observable("");

    vm.Change = ko.observable(0);
    vm.Branch = ko.observable("");
    vm.Today = ko.observable("Date: " + vm.FormatDate(new Date()));
    vm.RSS = ko.observable("");
    vm.First = new CurrencyModel();
    vm.Second = new CurrencyModel();
    vm.Third = new CurrencyModel();
    vm.Fourth = new CurrencyModel();
    vm.Fifth = new CurrencyModel();
    vm.Sixth = new CurrencyModel();
    vm.Seven = new CurrencyModel();
    vm.Eight = new CurrencyModel();
    vm.FirstNews = new CurrencyModel();

    vm.FullScreen = ko.observable(false);
    vm.Step = ko.observable(0);
    vm.Year = ko.observable(vm.GetYear(new Date()));
    vm.BranchInAmharic = ko.observable();
    vm.ShortLengthId = ko.observable(0);
    vm.LongLengthId = ko.observable(0);
    vm.LongVideo = ko.observable("");
    vm.ShortVideo = ko.observable("");
    vm.LongVideoLength = ko.observable(0);
    vm.ChangeFullScreen = ko.observable(false);
    vm.Template = ko.observable("");
    vm.Orientation = ko.observable("");
    vm.Rotate = ko.observable(true);
    vm.AmharicCalendar = ko.observable("");
    vm.LongInterval = ko.observable();
    vm.ShortInterval = ko.observable();
    vm.Seconds = ko.observable();
    vm.FullInterval = ko.observable();
    vm.CurrencyPosition = ko.observable("");
    vm.Calendar = ko.observable("");
    vm.ShowFeature = ko.observable("Change");
    vm.FeatureDuration = ko.observable(0);
    vm.FeatureInterval = ko.observable("");
    vm.Hour = ko.observable("");
    vm.Minute = ko.observable("");
    vm.Seconds = ko.observable("");
    vm.TimeInterval = ko.observable("");
    vm.TimeText = ko.observable("ጠዋት");
    vm.Time = ko.observable("");
    vm.TimeDuration = ko.observable(0);
    vm.WeatherDuration = ko.observable(0);
    vm.ChangeDuration = ko.observable(0);
    vm.NewsDuration = ko.observable(0);
    vm.ContentDuration = ko.observable(0);
    vm.AMPM = ko.observable();
    vm.NewsTicker = ko.observable();
    vm.LogoLeft = ko.observable();
    vm.LeftLogoWidth = ko.observable();
    vm.RightLogoWidth = ko.observable();
    vm.QueueDisplay = ko.observable();
    vm.HasQueue = ko.observable(false);

    vm.countDateInterval = ko.observable();
    vm.activate = function () {
        vm.startToListen();
        vm.GetData();
        vm.GetQueryDisplay();
        vm.countDateInterval(setInterval(vm.getCountDate, 1000));

        if (vm.LongVideoLength() > 0)
            vm.LongInterval(setInterval(vm.ChangeFullScreenVideo, vm.LongVideoLength() * 1000));

        //vm.GetVideoLength();
        //var myVar = setInterval(vm.GetData, 10000);
        //var myVarRSS = setInterval(vm.RotateRSS, 10000);
        vm.SetFeatureInterval();
        vm.ChangeTime();
        vm.RotateRSS();

        vm.router.map([
        ]).buildNavigationModel()
            .mapUnknownRoutes('', 'digital-signage')
            .activate();


       // dd.options['visible'] = 3;

    }
    vm.FeatureCounter = ko.observable("");
    vm.FeatureToken = ko.observable("");
    vm.QueryId = ko.observable("");
    vm.startToListen = function() {
        this.connection = new signalr.HubConnectionBuilder()
            //   .withUrl('taskOverviews')
            .withUrl('Queues')
            .configureLogging(signalr.LogLevel.Information)
            .build();
        this.connection.start().catch(function(err) {
            return console.error(err.toString());
        });
        this.connection.on("Queue", function (data) {
            vm.GetQueryDisplay();
            if (data == "fade" || data == "first-missing") {
                vm.FeatureCounter(vm.QueueDisplay().FirstCounter);
                vm.FeatureToken(vm.QueueDisplay().FirstToken);
                vm.QueryId("#queue,#first-queue");
                vm.FadeQueueDisplay();
            }
            else if (data == "second-missing") {
                vm.FeatureCounter(vm.QueueDisplay().SecondCounter);
                vm.FeatureToken(vm.QueueDisplay().SecondToken);
                vm.QueryId("#queue,#second-queue");
                vm.FadeQueueDisplay();
            }
            else if (data == "third-missing") {
                vm.FeatureCounter(vm.QueueDisplay().ThirdCounter);
                vm.FeatureToken(vm.QueueDisplay().ThirdToken);
                vm.QueryId("#queue,#third-queue");
                vm.FadeQueueDisplay();
            }
            else if (data == "fourth-missing") {
                vm.FeatureCounter(vm.QueueDisplay().FourthCounter);
                vm.FeatureToken(vm.QueueDisplay().FourthToken);
                vm.QueryId("#queue,#fourth-queue");
                vm.FadeQueueDisplay();
            }
            else if (data == "fifth-missing") {
                vm.FeatureCounter(vm.QueueDisplay().FifthCounter);
                vm.FeatureToken(vm.QueueDisplay().FifthToken);
                vm.QueryId("#queue,#fifth-queue");
                vm.FadeQueueDisplay();
            }
            else if (data == "sixth-missing") {
                vm.FeatureCounter(vm.QueueDisplay().SixthCounter);
                vm.FeatureToken(vm.QueueDisplay().SixthToken);
                vm.QueryId("#queue,#sixth-queue");
                vm.FadeQueueDisplay();
            }
            else if (data == "seventh-missing") {
                vm.FeatureCounter(vm.QueueDisplay().SeventhCounter);
                vm.FeatureToken(vm.QueueDisplay().SeventhToken);
                vm.QueryId("#queue,#seventh-queue");
                vm.FadeQueueDisplay();
            }
        });
    }

    vm.FadeQueueDisplay = function () {
        vm.HasQueue(true);
        $(vm.QueryId()).delay(500).fadeOut(500, function () {

        }).fadeIn(500);
        $(vm.QueryId()).delay(500).fadeOut(500, function () {

        }).fadeIn(500);
        $(vm.QueryId()).delay(500).fadeOut(500, function () {

        }).fadeIn(500);
        $(vm.QueryId()).delay(500).fadeOut(500, function () {

        }).fadeIn(500);
        $(vm.QueryId()).delay(500).fadeOut(500, function () {
            vm.HasQueue(false);
        }).fadeIn(500);
        return;
    }

    vm.compositionComplete = function () {
        vm.ChangeSmallScreenVideo();
    }

    function getAMPM(date) {
        var hours = date.getHours();
        var ampm = hours >= 12 ? 'PM' : 'AM';
        return ampm;
    }

    function getHour(date) {
        var hours = date.getHours();
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        hours = ("0" + hours).slice(-2);
        return hours;
    }

    vm.ChangeTime = function () {
        var d = new Date();
        var t = d.toLocaleTimeString();
        vm.Time(t);
        d.getUTCHours();
        vm.Hour(getHour(d));
        vm.Minute(d.getMinutes());
        vm.Minute(("0" + vm.Minute()).slice(-2));
        vm.Seconds(d.getSeconds());
        vm.Seconds(("0" + vm.Seconds()).slice(-2));

        vm.AMPM(getAMPM(d));
        clearInterval(vm.TimeInterval());
        vm.TimeInterval(setInterval(vm.ChangeTime, 1000));
    }

    vm.SetFeatureInterval = function () {
        if (vm.ShowFeature() == "Time") {
            if (vm.WeatherDuration() > 0) {
                vm.ShowFeature("Weather");
                vm.FeatureDuration(vm.WeatherDuration());
            }
            else if (vm.ChangeDuration() > 0) {
                vm.ShowFeature("Change");
                vm.FeatureDuration(vm.ChangeDuration());

            }
            else if (vm.NewsDuration() > 0) {
                vm.ShowFeature("News");
                vm.FeatureDuration(vm.NewsDuration());
            }
            else if (vm.TimeDuration() > 0) {
                vm.ShowFeature("Time");
                vm.FeatureDuration(vm.TimeDuration());
            }

            else {
                vm.ShowFeature("None");
                vm.FeatureDuration(10);
            }
        }
        else if (vm.ShowFeature() == "Weather") {
            if (vm.ChangeDuration() > 0) {
                vm.ShowFeature("Change");
                vm.FeatureDuration(vm.ChangeDuration());

            }
            else if (vm.NewsDuration() > 0) {
                vm.ShowFeature("News");
                vm.FeatureDuration(vm.NewsDuration());
            }
            else if (vm.TimeDuration() > 0) {
                vm.ShowFeature("Time");
                vm.FeatureDuration(vm.TimeDuration());
            }

            else if (vm.WeatherDuration() > 0) {
                vm.ShowFeature("Weather");
                vm.FeatureDuration(vm.WeatherDuration());
            }
            else {
                vm.ShowFeature("None");
                vm.FeatureDuration(10);
            }
        }
        else if (vm.ShowFeature() == "Change") {
            if (vm.NewsDuration() > 0) {
                vm.ShowFeature("News");
                vm.FeatureDuration(vm.NewsDuration());
            }

            else if (vm.TimeDuration() > 0) {
                vm.ShowFeature("Time");
                vm.FeatureDuration(vm.TimeDuration());
            }
            else if (vm.WeatherDuration() > 0) {
                vm.ShowFeature("Weather");
                vm.FeatureDuration(vm.WeatherDuration());
            }
            else if (vm.ChangeDuration() > 0) {
                vm.ShowFeature("Change");
                vm.FeatureDuration(vm.ChangeDuration());

            }
        }
        else if (vm.ShowFeature() == "News") {

                if (vm.TimeDuration() > 0) {
                    vm.ShowFeature("Time");
                    vm.FeatureDuration(vm.TimeDuration());
                }
                else if (vm.WeatherDuration() > 0) {
                    vm.ShowFeature("Weather");
                    vm.FeatureDuration(vm.WeatherDuration());
                }

                else if (vm.ChangeDuration() > 0) {
                    vm.ShowFeature("Change");
                    vm.FeatureDuration(vm.ChangeDuration());

                }
            else if (vm.NewsDuration() > 0) {
                vm.ShowFeature("News");
                vm.FeatureDuration(vm.NewsDuration());
            }


            else {
                vm.ShowFeature("None");
                vm.FeatureDuration(10);
            }
        }
        else {
            if (vm.TimeDuration() > 0) {
                vm.ShowFeature("Time");
                vm.FeatureDuration(vm.TimeDuration());
            }
            else if (vm.WeatherDuration() > 0) {
                vm.ShowFeature("Weather");
                vm.FeatureDuration(vm.WeatherDuration());
            }
            else if (vm.ChangeDuration() > 0) {
                vm.ShowFeature("Change");
                vm.FeatureDuration(vm.ChangeDuration());

            }
            else if (vm.NewsDuration() > 0) {
                vm.ShowFeature("News");
                vm.FeatureDuration(vm.NewsDuration());
            }

            else {
                vm.ShowFeature("None");
                vm.FeatureDuration(10);
            }
        }

        clearInterval(vm.FeatureInterval());
        vm.FeatureInterval(setInterval(vm.SetFeatureInterval, vm.FeatureDuration() * 1000));
    }
    vm.RSSLogo = ko.observable("");
    vm.RSSFirst = ko.observable(true);
    vm.RotateInterval = ko.observable();
    vm.RotateRSS = function () {
        $.ajax({
            async: false,
            cache: false,
            url: "api/Imports/GetRSSNews/" + vm.RSSLength(),
            type: "GET",
            contentType: "application/json; charset=utf-8",
            data: {},
            success: function (data) {
                vm.RSSStyle(data.RSSStyleView);
                vm.RSSLength(data.RSSLength);
                vm.RSSNews(data.RSSNewsView);

                if (vm.RSSLogo() == "") {
                    vm.RSSText(vm.RSSNews().NewsContent);
                    vm.RSSCategory(vm.RSSStyle().Category);
                    vm.RSSDate(vm.RSSNews().Date);
                    vm.RSSTitle(vm.RSSNews().Title);
                    vm.RSSLogo(data.RSSStyleView.LogoPath);
                }
                else if (vm.RSSLogo() != vm.RSSStyle().LogoPath) {
                    var el = '';
                    if (vm.CurrencyPosition() == 'Left')
                        el = $('.news-left');
                    else if (vm.CurrencyPosition() == 'Right')
                        el = $('.news-right');

                    var newone = el.clone(true);
                    el.before(newone);

                    el.remove();

                    $('.news-div').css({

                        "-webkit-animation-name": "slidein",
                        "-webkit-animation-duration": "0.8s",

                        //  "-moz-animation-name": "slidein",
                        //   "-moz-animation-duration": "0.8s",

                    });
                    if (vm.CurrencyPosition() == 'Left') {
                        ko.cleanNode(document.getElementById('element-left'));
                        ko.applyBindings(vm, document.getElementById('element-left'));
                    }
                    else if (vm.CurrencyPosition() == 'Right') {
                        ko.cleanNode(document.getElementById('element-right'));
                        ko.applyBindings(vm, document.getElementById('element-right'));
                    }


                    $(".RSSText").fadeOut(500, function () {
                        vm.RSSText(vm.RSSNews().NewsContent);
                        vm.RSSCategory(vm.RSSStyle().Category);
                        vm.RSSDate(vm.RSSNews().Date);
                        vm.RSSTitle(vm.RSSNews().Title);
                        vm.RSSLogo(data.RSSStyleView.LogoPath);


                    }).fadeIn(500);
                }
                else {
                    $(".RSSText").fadeOut(500, function () {
                        vm.RSSText(vm.RSSNews().NewsContent);
                        vm.RSSCategory(vm.RSSStyle().Category);
                        vm.RSSDate(vm.RSSNews().Date);
                        vm.RSSTitle(vm.RSSNews().Title);
                        vm.RSSLogo(data.RSSStyleView.LogoPath);

                    }).fadeIn(500);
                }

               // clearInterval(vm.rssInterval());
               // vm.scheduleInterval(setInterval(vm.RotateRSS, 15000));
            }
        }).fail(function (xhr, textStatus, err) {
            return false;
        });

        clearInterval(vm.RSSInterval());
        vm.RSSInterval(setInterval(vm.RotateRSS, vm.ContentDuration() * 1000));
       // $('.news-div').animate();
        //vm.ChangeRSS();

    }
    //vm.ChangeRSS = function () {

    //   // var wrapper = window.document.querySelector(".news");


    //}
    vm.ShortInterval = ko.observable();
    vm.SmallSeconds = ko.observable();
    vm.VideoProblem = ko.observable();
    vm.ChangeSmallScreenVideo = function () {
        var sec = 0;
        $.ajax({
            async: false,
            cache: false,
            url: "api/Imports/GetVideoLength/" + vm.ShortLengthId() + "/0",
            type: "GET",
            contentType: "application/json; charset=utf-8",
            data: {},
            success: function (data) {

                sec = data.Length;
                vm.Seconds(data.Length);
                vm.SmallSeconds(data.Length);
                vm.ShortLengthId(data.Step);
                vm.ShortVideo(data.Name);
                vm.IsVideo(data.IsVideo);
                //Streaming
                vm.IsLiveStream(data.IsStream);
                
                vm.OldIsVideo(data.IsVideo);

                var longVideo = document.getElementById('smallScreen');
                var outdoorVideo = document.getElementById('fullScreen-outdoor');

                if (outdoorVideo != null) {
                    longVideo = outdoorVideo;
                    longVideo.pause();
                    longVideo.removeAttribute('src');
                    longVideo.load();
                    longVideo.setAttribute('src', 'lib/Project/Video/' + data.Name);
                }
                else {
                    if (longVideo != null) {
                        longVideo.pause();
                        longVideo.removeAttribute('src');
                        longVideo.load();
                        if (vm.IsVideo()) {
                            longVideo.setAttribute('src', 'lib/Project/Video/' + data.Name);
                            if (vm.FullScreen()) {
                                longVideo.setAttribute('id', 'smallScreen');
                            }

                        }
                    }
                    else {

                    }

                }

            }
        }).fail(function (xhr, textStatus, err) {
            return false;
        });
        clearInterval(vm.ShortInterval());
        vm.ShortInterval(setInterval(vm.ChangeSmallScreenVideo, sec * 1000));
    }

    vm.OldIsVideo = ko.observable(false);
    vm.ChangeFullScreenVideo = function () {
        var sec = 0;
        $.ajax({
            async: false,
            cache: false,
            url: "api/Imports/GetVideoLength/" + vm.LongLengthId() + "/1",
            type: "GET",
            contentType: "application/json; charset=utf-8",
            data: {},
            success: function (data) {
                sec = data.Length;
                vm.Seconds(data.Length);
                vm.LongLengthId(data.Step);

                vm.IsVideo(true);
                var longVideo = document.getElementById('smallScreen');
                var fullVideo = document.getElementById('fullScreen');
                var outdoorVideo = document.getElementById('fullScreen-outdoor');

                if (outdoorVideo != null) {
                    longVideo = outdoorVideo;

                    longVideo.pause();
                    longVideo.removeAttribute('src');
                    longVideo.load();
                    longVideo.setAttribute('src', 'lib/Project/Video/' + data.Name);
                }
                else {
                    if (fullVideo != null)
                        longVideo = fullVideo;

                    if (longVideo != null) {
                        longVideo.pause();
                        longVideo.removeAttribute('src');
                        longVideo.load();
                        longVideo.setAttribute('src', 'lib/Project/Video/' + data.Name);
                        longVideo.setAttribute('id', 'fullScreen');

                        //if (vm.Orientation() == 'Landscape')
                        //if (vm.Orientation() == 'Portrait')
                        //    longVideo.setAttribute('id', 'smallScreen');
                    }
                }


            }
        }).fail(function (xhr, textStatus, err) {
            return false;
            });
        clearInterval(vm.ShortInterval());
        vm.ShortInterval(setInterval(vm.SetShortVideo, sec * 1000));

        clearInterval(vm.LongInterval());
        vm.LongInterval(setInterval(vm.ChangeFullScreenVideo, vm.LongVideoLength() * 1000 + (sec * 1000)));
    }

    vm.SetShortVideo = function () {
        var longVideo = document.getElementById('smallScreen');
        var fullVideo = document.getElementById('fullScreen');
        var outdoorVideo = document.getElementById('fullScreen-outdoor');
        if (outdoorVideo != null) {
            longVideo = outdoorVideo;

            longVideo.pause();
            longVideo.removeAttribute('src');
            longVideo.setAttribute('src', '');
            longVideo.setAttribute('id', 'fullScreen-outdoor');

            longVideo.load();
        }
        else {
            if (fullVideo != null)
                longVideo = fullVideo;
            

            if (longVideo != null) {

                longVideo.pause();
                longVideo.removeAttribute('src');
                longVideo.setAttribute('src', '');
                longVideo.setAttribute('id', 'smallScreen');
                longVideo.load();

                //For Left and Right
                if (vm.CurrencyPosition() == 'Left') {
                    vm.CurrencyPosition("Right");
                }
                else if (vm.CurrencyPosition() == 'Right') {
                    vm.CurrencyPosition("Left");

                }
                else if (vm.CurrencyPosition() == 'Top') {
                    vm.CurrencyPosition("Bottom");

                }
                else if (vm.CurrencyPosition() == 'Bottom') {
                    vm.CurrencyPosition("Top");

                }

                //longVideo.setAttribute('src', 'lib/Project/Video/' + vm.ShortVideo());
                //longVideo.setAttribute('id', 'smallScreen');
                //For Left Only
                vm.IsVideo(vm.OldIsVideo());
                //if (vm.OldIsVideo()) {
                //    longVideo.setAttribute('src', 'lib/Project/Video/' + vm.ShortVideo());
                //    if (vm.FullScreen()) {
                //        longVideo.setAttribute('id', 'smallScreen');
                //    }
                //}

                //For Left and Right
                if (vm.OldIsVideo()) {
                    //longVideo.setAttribute('src', 'lib/Project/Video/' + vm.ShortVideo());
                    if (vm.FullScreen()) {
                        var longVideo = document.getElementById('smallScreen');
                        var fullVideo = document.getElementById('fullScreen');
                        var outdoorVideo = document.getElementById('fullScreen-outdoor');
                        if (outdoorVideo != null) {
                            longVideo = outdoorVideo;
                            longVideo.setAttribute('src', 'lib/Project/Video/' + vm.ShortVideo());
                        }
                        else {
                            if (fullVideo != null)
                                longVideo = fullVideo;
                            if (longVideo != null) {
                                longVideo.setAttribute('id', 'smallScreen');
                                longVideo.setAttribute('src', 'lib/Project/Video/' + vm.ShortVideo());
                            }
                        }

                    }
                }
        }
        
        }
        clearInterval(vm.ShortInterval());
        vm.ShortInterval(setInterval(vm.ChangeSmallScreenVideo, vm.SmallSeconds() * 1000));
    }

    vm.IsVideo = ko.observable(false);
    vm.GetVideoLength = function () {
        console.log('get video length called');
        if (vm.FullScreen()) {
            console.log(shortVideo + ' full:' + fullVideo);
            clearInterval(vm.FullInterval());
            vm.FullInterval(setInterval(vm.GetVideoLength, vm.LongVideoLength() * 1000));
            var longVideo = document.getElementById('smallScreen');
            var fullVideo = document.getElementById('fullScreen');
            if (longVideo != null) {
                console.log('pausing long video');
                longVideo.pause();
                longVideo.removeAttribute('src');
                longVideo.load();
            }
            else {
                console.log('not getting small screen');
            }

        }
        else {
            var fullVideo = document.getElementById('fullScreen');
            var shortVideo = document.getElementById('smallScreen');
            console.log(fullVideo + ' short:' + shortVideo);

            var sec = 0;
            $.ajax({
                async: false,
                cache: false,
                url: "api/Imports/GetVideoLength/" + vm.ShortLengthId() + "/1",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                data: {},
                success: function (data) {
                    vm.Seconds(data.Length);
                    sec = data.Length;
                    vm.IsVideo(data.IsVideo);
                    vm.ShortLengthId(data.Step);
                    vm.ShortVideo(data.Name);
                    //Streaming
                    vm.IsLiveStream(data.IsStream);

                    vm.LongVideo("");
                }
            }).fail(function (xhr, textStatus, err) {
                return false;
            });
            var longVideo = document.getElementById('smallScreen');
            var fullVideo = document.getElementById('fullScreen');
            if (longVideo != null) {
                longVideo.pause();
                longVideo.removeAttribute('src');
                longVideo.load();
            }
            else {
                console.log('not getting small screen');
            }
            clearInterval(vm.FullInterval());
            vm.FullInterval(setInterval(vm.GetVideoLength, sec * 1000));
        }
    }

    vm.NavStyle = ko.observable();
    vm.BodyStyle = ko.observable();
    vm.scheduleInterval = ko.observable();
    vm.reload = ko.observable(false);
    vm.ScheduleId = ko.observable(0);

    vm.GetQueryDisplay = function () {
        $.ajax({
            async: false,
            cache: false,
            url: "api/Queues/GetQueryDisplay",
            type: "GET",
            contentType: "application/json; charset=utf-8",
            data: {},
            success: function (data) {
                vm.QueueDisplay(data);
            }

        }).fail(function (xhr, textStatus, err) {
            return false;
        });
    }

    vm.GetData = function () {
        $.ajax({
            async: false,
            cache: false,
            url: "api/Imports/GetData/" + vm.Step(),
            type: "GET",
            contentType: "application/json; charset=utf-8",
            data: {},
            success: function (data) {
                vm.countDate(data.ScheduleViews[0].EndDate);
                vm.NavStyle(data.NavStyle);
                vm.LogoLeft(vm.NavStyle().LogoLeft);
                vm.LeftLogoWidth(vm.NavStyle().LeftLogoWidth);
                vm.RightLogoWidth(vm.NavStyle().RightLogoWidth);
                vm.BodyStyle(data.BodyStyle);
                vm.WeatherLogoPath("lib/Project/img/" + vm.BodyStyle().WeatherLogoPath);
                if (vm.ScheduleId() == 0)
                    vm.ScheduleId(data.ScheduleViews[0].Id);
                else if (vm.ScheduleId() != data.ScheduleViews[0].Id) {
                    vm.reload(true);
                }
                vm.LongVideoLength(data.ScheduleViews[0].VideoLength);
                vm.TimeDuration(data.ScheduleViews[0].TimeDuration);
                vm.ChangeDuration(data.ScheduleViews[0].DifferenceDuration);
                vm.WeatherDuration(data.ScheduleViews[0].WeatherDuration);
                vm.NewsDuration(data.ScheduleViews[0].NewsDuration);
                vm.ContentDuration(data.ScheduleViews[0].ContentDuration);
                vm.Calendar(data.CalendarView);
                vm.Step(data.Step);
                vm.AmharicCalendar("ቀን: " + data.AmharicCalendar);
                vm.Today("Date: " + data.GregorianCalendar);



                if (vm.CurrencyPosition() == '' || vm.Template() != data.ScheduleViews[0].Template) {
                    vm.CurrencyPosition(data.ScheduleViews[0].CurrencyPosition);
                    
                }


                vm.Template(data.ScheduleViews[0].Template);
                vm.Orientation(data.ScheduleViews[0].Orientation);
                if (vm.Change() == 0) {
                    if (data.CurrencyViews.length > 0) {
                        vm.First.Model().Currency(data.CurrencyViews[0].Currency);
                        vm.First.Model().Abbreviation(data.CurrencyViews[0].Abbreviation);
                        vm.First.Model().Difference(data.CurrencyViews[0].Difference);
                        vm.First.Model().Flag(data.CurrencyViews[0].Flag);
                        vm.First.Model().Buying(data.CurrencyViews[0].Buying);
                        vm.First.Model().Selling(data.CurrencyViews[0].Selling);
                        vm.First.Model().UpdateDate(data.CurrencyViews[0].UpdateDate);

                    }
                    if (data.CurrencyViews.length > 1) {
                        vm.Second.Model().Currency(data.CurrencyViews[1].Currency);
                        vm.Second.Model().Abbreviation(data.CurrencyViews[1].Abbreviation);
                        vm.Second.Model().Difference(data.CurrencyViews[1].Difference);
                        vm.Second.Model().Flag(data.CurrencyViews[1].Flag);
                        vm.Second.Model().Buying(data.CurrencyViews[1].Buying);
                        vm.Second.Model().Selling(data.CurrencyViews[1].Selling);
                        vm.Second.Model().UpdateDate(data.CurrencyViews[1].UpdateDate);

                    }
                    if (data.CurrencyViews.length > 2) {
                        vm.Third.Model().Currency(data.CurrencyViews[2].Currency);
                        vm.Third.Model().Abbreviation(data.CurrencyViews[2].Abbreviation);
                        vm.Third.Model().Difference(data.CurrencyViews[2].Difference);
                        vm.Third.Model().Flag(data.CurrencyViews[2].Flag);
                        vm.Third.Model().Buying(data.CurrencyViews[2].Buying);
                        vm.Third.Model().Selling(data.CurrencyViews[2].Selling);
                        vm.Third.Model().UpdateDate(data.CurrencyViews[2].UpdateDate);

                    }
                    if (data.CurrencyViews.length > 3) {
                        vm.Fourth.Model().Currency(data.CurrencyViews[3].Currency);
                        vm.Fourth.Model().Abbreviation(data.CurrencyViews[3].Abbreviation);
                        vm.Fourth.Model().Difference(data.CurrencyViews[3].Difference);
                        vm.Fourth.Model().Flag(data.CurrencyViews[3].Flag);
                        vm.Fourth.Model().Buying(data.CurrencyViews[3].Buying);
                        vm.Fourth.Model().Selling(data.CurrencyViews[3].Selling);
                        vm.Fourth.Model().UpdateDate(data.CurrencyViews[3].UpdateDate);

                    }
                    if (data.CurrencyViews.length > 4) {
                        vm.Fifth.Model().Currency(data.CurrencyViews[4].Currency);
                        vm.Fifth.Model().Abbreviation(data.CurrencyViews[4].Abbreviation);
                        vm.Fifth.Model().Difference(data.CurrencyViews[4].Difference);
                        vm.Fifth.Model().Flag(data.CurrencyViews[4].Flag);
                        vm.Fifth.Model().Buying(data.CurrencyViews[4].Buying);
                        vm.Fifth.Model().Selling(data.CurrencyViews[4].Selling);
                        vm.Fifth.Model().UpdateDate(data.CurrencyViews[4].UpdateDate);

                    }

                    if (data.CurrencyViews.length > 5) {
                        vm.Sixth.Model().Currency(data.CurrencyViews[5].Currency);
                        vm.Sixth.Model().Abbreviation(data.CurrencyViews[5].Abbreviation);
                        vm.Sixth.Model().Difference(data.CurrencyViews[5].Difference);
                        vm.Sixth.Model().Flag(data.CurrencyViews[5].Flag);
                        vm.Sixth.Model().Buying(data.CurrencyViews[5].Buying);
                        vm.Sixth.Model().Selling(data.CurrencyViews[5].Selling);
                        vm.Sixth.Model().UpdateDate(data.CurrencyViews[5].UpdateDate);

                    }

                    if (data.CurrencyViews.length > 6) {
                        vm.Seven.Model().Currency(data.CurrencyViews[6].Currency);
                        vm.Seven.Model().Abbreviation(data.CurrencyViews[6].Abbreviation);
                        vm.Seven.Model().Difference(data.CurrencyViews[6].Difference);
                        vm.Seven.Model().Flag(data.CurrencyViews[6].Flag);
                        vm.Seven.Model().Buying(data.CurrencyViews[6].Buying);
                        vm.Seven.Model().Selling(data.CurrencyViews[6].Selling);
                        vm.Seven.Model().UpdateDate(data.CurrencyViews[6].UpdateDate);

                    }

                    if (data.CurrencyViews.length > 7) {
                        vm.Eight.Model().Currency(data.CurrencyViews[7].Currency);
                        vm.Eight.Model().Abbreviation(data.CurrencyViews[7].Abbreviation);
                        vm.Eight.Model().Difference(data.CurrencyViews[7].Difference);
                        vm.Eight.Model().Flag(data.CurrencyViews[7].Flag);
                        vm.Eight.Model().Buying(data.CurrencyViews[7].Buying);
                        vm.Eight.Model().Selling(data.CurrencyViews[7].Selling);
                        vm.Eight.Model().UpdateDate(data.CurrencyViews[7].UpdateDate);

                    }
                    vm.Change(1);
                }
                else {
                    $("#one,#one-difference").delay(1000).fadeOut(500, function () {
                        if (data.CurrencyViews.length > 0) {
                            vm.First.Model().Currency(data.CurrencyViews[0].Currency);
                            vm.First.Model().Abbreviation(data.CurrencyViews[0].Abbreviation);
                            vm.First.Model().Difference(data.CurrencyViews[0].Difference);
                            vm.First.Model().Flag(data.CurrencyViews[0].Flag);
                            vm.First.Model().Buying(data.CurrencyViews[0].Buying);
                            vm.First.Model().Selling(data.CurrencyViews[0].Selling);
                            vm.First.Model().UpdateDate(data.CurrencyViews[0].UpdateDate);

                        }
                    }).fadeIn(500);

                    $("#two,#two-difference").delay(2000).fadeOut(500, function () {
                        if (data.CurrencyViews.length > 1) {
                            vm.Second.Model().Currency(data.CurrencyViews[1].Currency);
                            vm.Second.Model().Abbreviation(data.CurrencyViews[1].Abbreviation);
                            vm.Second.Model().Difference(data.CurrencyViews[1].Difference);
                            vm.Second.Model().Flag(data.CurrencyViews[1].Flag);
                            vm.Second.Model().Buying(data.CurrencyViews[1].Buying);
                            vm.Second.Model().Selling(data.CurrencyViews[1].Selling);
                            vm.Second.Model().UpdateDate(data.CurrencyViews[1].UpdateDate);

                        }
                    }).fadeIn(500);

                    $("#three,#three-difference").delay(3000).fadeOut(500, function () {
                        if (data.CurrencyViews.length > 2) {
                            vm.Third.Model().Currency(data.CurrencyViews[2].Currency);
                            vm.Third.Model().Abbreviation(data.CurrencyViews[2].Abbreviation);
                            vm.Third.Model().Difference(data.CurrencyViews[2].Difference);
                            vm.Third.Model().Flag(data.CurrencyViews[2].Flag);
                            vm.Third.Model().Buying(data.CurrencyViews[2].Buying);
                            vm.Third.Model().Selling(data.CurrencyViews[2].Selling);
                            vm.Third.Model().UpdateDate(data.CurrencyViews[2].UpdateDate);

                        }
                    }).fadeIn(500);

                    $("#four,#four-difference").delay(4000).fadeOut(500, function () {
                        if (data.CurrencyViews.length > 3) {
                            vm.Fourth.Model().Currency(data.CurrencyViews[3].Currency);
                            vm.Fourth.Model().Abbreviation(data.CurrencyViews[3].Abbreviation);
                            vm.Fourth.Model().Difference(data.CurrencyViews[3].Difference);
                            vm.Fourth.Model().Flag(data.CurrencyViews[3].Flag);
                            vm.Fourth.Model().Buying(data.CurrencyViews[3].Buying);
                            vm.Fourth.Model().Selling(data.CurrencyViews[3].Selling);
                            vm.Fourth.Model().UpdateDate(data.CurrencyViews[3].UpdateDate);

                        }
                    }).fadeIn(500);

                    $("#five,#five-difference").delay(5000).fadeOut(500, function () {
                        if (data.CurrencyViews.length > 4) {
                            vm.Fifth.Model().Currency(data.CurrencyViews[4].Currency);
                            vm.Fifth.Model().Abbreviation(data.CurrencyViews[4].Abbreviation);
                            vm.Fifth.Model().Difference(data.CurrencyViews[4].Difference);
                            vm.Fifth.Model().Flag(data.CurrencyViews[4].Flag);
                            vm.Fifth.Model().Buying(data.CurrencyViews[4].Buying);
                            vm.Fifth.Model().Selling(data.CurrencyViews[4].Selling);
                            vm.Fifth.Model().UpdateDate(data.CurrencyViews[4].UpdateDate);

                        }
                    }).fadeIn(500);

                    $("#six,#six-difference").delay(6000).fadeOut(500, function () {
                        if (data.CurrencyViews.length > 5) {
                            vm.Sixth.Model().Currency(data.CurrencyViews[5].Currency);
                            vm.Sixth.Model().Abbreviation(data.CurrencyViews[5].Abbreviation);
                            vm.Sixth.Model().Difference(data.CurrencyViews[5].Difference);
                            vm.Sixth.Model().Flag(data.CurrencyViews[5].Flag);
                            vm.Sixth.Model().Buying(data.CurrencyViews[5].Buying);
                            vm.Sixth.Model().Selling(data.CurrencyViews[5].Selling);
                            vm.Sixth.Model().UpdateDate(data.CurrencyViews[5].UpdateDate);

                        }
                    }).fadeIn(500);

                    $("#seven,#seven-difference").delay(7000).fadeOut(500, function () {
                        if (data.CurrencyViews.length > 6) {
                            vm.Seven.Model().Currency(data.CurrencyViews[6].Currency);
                            vm.Seven.Model().Abbreviation(data.CurrencyViews[6].Abbreviation);
                            vm.Seven.Model().Difference(data.CurrencyViews[6].Difference);
                            vm.Seven.Model().Flag(data.CurrencyViews[6].Flag);
                            vm.Seven.Model().Buying(data.CurrencyViews[6].Buying);
                            vm.Seven.Model().Selling(data.CurrencyViews[6].Selling);
                            vm.Seven.Model().UpdateDate(data.CurrencyViews[6].UpdateDate);

                        }
                    }).fadeIn(500);

                    $("#eight,#eight-difference").delay(8000).fadeOut(500, function () {
                        if (data.CurrencyViews.length > 7) {
                            vm.Eight.Model().Currency(data.CurrencyViews[7].Currency);
                            vm.Eight.Model().Abbreviation(data.CurrencyViews[7].Abbreviation);
                            vm.Eight.Model().Difference(data.CurrencyViews[7].Difference);
                            vm.Eight.Model().Flag(data.CurrencyViews[7].Flag);
                            vm.Eight.Model().Buying(data.CurrencyViews[7].Buying);
                            vm.Eight.Model().Selling(data.CurrencyViews[7].Selling);
                            vm.Eight.Model().UpdateDate(data.CurrencyViews[7].UpdateDate);

                          //  setTimeout(2000);
                          //  vm.GetData();

                        }
                    }).fadeIn(500);
                }




                if (vm.LongVideoLength() > 0)
                    vm.FullScreen(true);
                vm.Branch(data.ScheduleViews[0].Branch);
                vm.RSS(data.ScheduleViews[0].RSS);
                vm.BranchInAmharic(data.ScheduleViews[0].BranchInAmharic);
                if (vm.Rotate()) {
                    vm.Rotate(false);
                }
                else {
                    vm.Rotate(true);
                }

                if (vm.reload()) {
                    vm.reload(false);
                    location.reload();
                }
                clearInterval(vm.scheduleInterval());
                vm.scheduleInterval(setInterval(vm.GetData, 15000));

            }



        }).fail(function (xhr, textStatus, err) {
            return false;
            });


    }

    return vm;
});