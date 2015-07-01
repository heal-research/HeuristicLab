// main app as plugin
var appMainPlugin = app.registerPlugin(appName);
appMainPlugin.dependencies = ['oc.lazyLoad', 'ui.router', 'angular-loading-bar', 'ngResource'];
appMainPlugin.load();
(function () {
    'use strict';
    var module = appMainPlugin.getAngularModule();
    module.config(['$ocLazyLoadProvider', function ($ocLazyLoadProvider) {
        app.plugins.forEach(function (plugin) {
            $ocLazyLoadProvider.config({
                modules: [{
                    name: plugin.getFullPluginName(),
                    files: plugin.getFiles()
                }]
            });
        });
    }]);

    module.config([
        '$stateProvider', '$urlRouterProvider',
        function ($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise('/status');
            // load module routes
            app.plugins.forEach(function (plugin) {
                // home route
                $stateProvider.state(plugin.name, {
                    url: plugin.getRouteName(),
                    controller: plugin.controller,
                    templateUrl: plugin.getViewUrl(plugin.view),
                    cache: false,
                    resolve: {
                        loadModule: ['$ocLazyLoad', 'cfpLoadingBar', function ($ocLazyLoad, cfpLoadingBar) {
                            cfpLoadingBar.start();
                            var retVal = plugin.load($ocLazyLoad);
                            cfpLoadingBar.complete();
                            return retVal;
                        }]
                    }
                });
                // sub-routes
                plugin.configureRoutes($stateProvider);
            });
        }
    ]);

    module.config([
        '$httpProvider', function ($httpProvider) {
            $httpProvider.interceptors.push(['$q', function ($q) {
                return {
                    'request': function (config) {
                        if (endsWith(config.url, '.cshtml')) {
                            config.url = 'App/LoadPluginView?' + config.url + "&dateTime=" + Date.now().toString();
                        }
                        return config;
                    }
                };
            }]);
        }
    ]);

    function endsWith(str, suffix) {
        if (suffix.length > str.length) {
            return false;
        }
        return str.indexOf(suffix, str.length - suffix.length) !== -1;
    }
})();