requirejs.config({
    urlArgs: 'v=' + new Date(),

    paths: {
        'text': '../lib/require/text',
        'durandal': '../lib/durandal/js',
        'plugins': '../lib/durandal/js/plugins',
        'transitions': '../lib/durandal/js/transitions',
        'knockout': '../lib/knockout/knockout-3.1.0',
        'bootstrap': '../lib/bootstrap/js/bootstrap',
        'jquery': '../lib/jquery/jquery-1.9.1',
        'notify': '../lib/bootstrap-notify/bootstrap-notify',
        'models': '../lib/Resource/Models',
        'koval': '../lib/knockout/knockout.validation',
        'script': '../lib/script',
        'shell': 'app/shell',
        'authentication': '../lib/Resource/Authentication',
        'kendo': '../lib/Kendo/kendo.all.min',
        'knockout-kendo': '../lib/Kendo/knockout-kendo.min',
        'permission': '../lib/Resource/Permission',
        'global-path': '../lib/Resource',
        'Services': '../lib/Resource/Services',
        'Extensions': '../lib/Resource/Extensions',
        'Entities': '../lib/Resource/Entities',
        'ViewModels': '../lib/Resource/Models',
        'Global': '../lib/Global',
        'ticker-easing': '../lib/news-ticker/jquery.easing.min',
        'ticker-easy': '../lib/news-ticker/jquery.easy-ticker.min',
        'ticker': '../lib/news-ticker/jquery.ticker',
        'Modules': 'app',
        'signalr': '../lib/signalr/signalr.min'

    },
    shim: {
        'bootstrap': {
            deps: ['jquery'],
            exports: '$'
        },
        'notify': {
            deps: ['bootstrap', 'jquery']
        },
        'models': {
            deps: ['knockout']
        },
        'koval': {
            deps: ['knockout']
        },
        'script': {
            deps: ['jquery', 'notify', 'knockout']
        },
        'filter': {
            deps: ['knockout']
        },
        'knockout-kendo': {
            deps: ['jquery', 'knockout', 'kendo']
        },
        'ticker-easy': {
            deps: ['jquery']
        },
        'ticker-easing': {
            deps: ['jquery']
        },
        'ticker': {
            deps: ['jquery']
        }

    }

});

define(['durandal/system', 'durandal/app', 'durandal/viewLocator'], function (system, app, viewLocator) {
    system.debug(true);

    app.title = 'Digital Signage';


    //specify which plugins to install and their configuration
    app.configurePlugins({
        router: true,
        dialog: true,
        widget: {
            kinds: ['expander']
        }
    });

    app.start().then(function () {
        app.setRoot('Shell');
    });
});