// app configuration
var appName = 'app';
var appPath = 'WebApp';
var defaultPageUpdateInterval = 60000;

// route, plugin, menu, section and main app
var Route = function (name, view, controller) {
    this.name = name;
    this.view = view;
    this.controller = controller;
};

var Plugin = function (pluginName) {
    this.name = pluginName;
    this.parent = appName;
    this.view = '';
    this.controller = '';
    this.dependencies = [];
    this.routes = [];
    this.files = [];

    this.getPluginDirectory = function () {
        return appPath + '/plugins/' + this.name + '/';
    };

    this.getFullPluginName = function() {
        if (this.name.localeCompare(appName) == 0) {
            return this.name;
        }
        return this.parent + '.' + this.name;
    };

    this.getRouteName = function() {
        return '/' + this.name;
    };

    this.getAngularModule = function() {
        return angular.module(this.getFullPluginName());
    };

    this.getFilePath = function (file) {
        return this.getPluginDirectory() + file;
    };

    this.getViewUrl = function (view) {
        return 'plugin=' + this.name + '&view=' + view;
    };

    // getFiles is used in the lazy loading provider 
    this.getFiles = function () {
        var plugin = this;
        var filesToLoad = [];
        plugin.files.forEach(function (file) {
            filesToLoad.push(plugin.getFilePath(file));
        });
        return filesToLoad;
    };

    this.configureRoutes = function ($stateProvider) {
        var plugin = this;
        this.routes.forEach(function (route) {
            $stateProvider.state(plugin.name + route.name, {
                url: '/' + plugin.name + '/' + route.name,
                controller: route.controller,
                templateUrl: plugin.getViewUrl(route.view),
                cache: false,
                resolve: {
                    loadPlugin: ['$ocLazyLoad', 'cfpLoadingBar', function ($ocLazyLoad, cfpLoadingBar) {
                        cfpLoadingBar.start();
                        var retVal = plugin.load($ocLazyLoad);
                        cfpLoadingBar.complete();
                        return retVal;
                    }]
                }
            });
        });
    };

    this.load = function ($ocLazyLoad) {
        var plugin = this;
        var params = [];
        var lazyLoadingFiles = [];
        plugin.dependencies.forEach(function (dependency) {
            params.push(dependency);
        });
        plugin.files.forEach(function (file) {
            lazyLoadingFiles.push(plugin.getFilePath(file));
        });
        // load required files
        var lazyLoaded = null;
        if (!($ocLazyLoad === undefined || $ocLazyLoad === null)) {
            lazyLoaded = $ocLazyLoad.load(plugin.getFullPluginName());
        }
        // load angular module
        angular.module(this.getFullPluginName(), params);
        return lazyLoaded;
    };
};

var Section = function (name, index) {
    this.name = name;
    this.index = index;
    this.entries = [];

    this.addEntry = function (entry) {
        this.entries.push(entry);
    };
};

var Menu = function () {
    this.sections = [];
    this.nextIndex = 0;

    this.getSection = function (name, index) {
        var length = this.sections.length;
        for (var i = 0; i < length; ++i) {
            var section = this.sections[i];
            if (section.name == name) {
                return section;
            }
        }
        if (index === undefined || index === null) {
            this.nextIndex++;
            index = this.nextIndex;
        } else {
            if (index > this.nextIndex) {
                this.nextIndex = index;
            }
        }
        var newSection = new Section(name, index);
        this.sections.push(newSection);
        return newSection;
    };

    this.getMenuEntries = function () {
        var entries = [];
        this.sections.sort(function (a, b) {
            return parseFloat(a.index) - parseFloat(b.index);
        });
        this.sections.forEach(function (section) {
            entries.push({ name: section.name, isCategory: true, route: '?' });
            section.entries.sort(function(a, b) {
                return parseFloat(a.index) - parseFloat(b.index);
            });
            section.entries.forEach(function (entry) {
                entries.push(entry);
            });
        });
        return entries;
    };
};

// app that holds references to plugins and menu
var app = new function () {
    this.plugins = [];
    this.registerPlugin = function (name) {
        var plugin = new Plugin(name);
        if (name.localeCompare(appName) != 0) {
            this.plugins.push(plugin);
        }
        return plugin;
    };

    this.menu = new Menu();
    this.getMenu = function () {
        return this.menu;
    };
};