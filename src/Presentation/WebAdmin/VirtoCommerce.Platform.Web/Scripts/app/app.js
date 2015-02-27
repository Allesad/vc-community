﻿var AppDependencies = [
  'ui.router',
  'ui.bootstrap',
  'ui.utils',
  'ui.sortable',
  'ui.select',
  'ngAnimate',
  'ngResource',
  'xeditable',
  'platformWebApp.dashboard',
  'platformWebApp.security',
  'platformWebApp.notification',
  'platformWebApp.mainMenu',
  'platformWebApp.bladeNavigation',
  'platformWebApp.overlay',
  'platformWebApp.contextMenu',
  'platformWebApp.Filters',
  'platformWebApp.htmlTooltip',
  'platformWebApp.widget',
  'platformWebApp.breadcrumbs',
  'platformWebApp.inputNumber'
];

angular.module('platformWebApp', AppDependencies).
  controller('appCtrl', ['$scope', '$state', '$http', 'mainMenuService', 'notificationService', function ($scope, $state, $http, mainMenuService, notificationService) {
	
	notificationService.run();

    $scope.curentStateName = function () {
        return $state.current.name;
    };
   
  }])
.factory('httpErrorInterceptor', ['$q', '$rootScope', function ($q, $rootScope) {
	var httpErrorInterceptor = {};

	httpErrorInterceptor.responseError = function (rejection) {
		if (rejection.status === 401) {
			$rootScope.$broadcast('unauthorized', rejection);
		}
		else {
			$rootScope.$broadcast('httpError', rejection);
		}
		return $q.reject(rejection);
	};
	httpErrorInterceptor.requestError = function (rejection) {
		$rootScope.$broadcast('httpError', rejection);
		return $q.reject(rejection);
	};

	return httpErrorInterceptor;
}])
.config(
  ['$stateProvider', '$httpProvider', 'uiSelectConfig', function ($stateProvider, $httpProvider, uiSelectConfig) {
    $stateProvider.state('workspace', {
						  abstract: true,
						  templateUrl: 'Scripts/app/workspace.tpl.html'
    });
	//Add interseptor
    $httpProvider.interceptors.push('httpErrorInterceptor');
	//ui-select set selectize as default theme
    uiSelectConfig.theme = 'select2';
  }
  ]
)
.run(
  ['$rootScope', '$state', '$stateParams', 'authService', 'mainMenuService', 'editableOptions', 'notificationService',
    function ($rootScope, $state, $stateParams, authService, mainMenuService, editableOptions, notificationService) {
        editableOptions.theme = 'bs3'; // bootstrap3 theme. Can be also 'bs2', 'default'

        $rootScope.$state = $state;
        $rootScope.$stateParams = $stateParams;
        var homeMenuItem = {
        	path: 'home',
        	title: 'Home',
        	icon: 'fa fa-home',
        	action: function () { $state.go('workspace.catalog') },
           	priority: 0
        };
        mainMenuService.addMenuItem(homeMenuItem);

        var menuItem = {
        	path: 'browse',
        	icon: 'fa fa-search',
        	title: 'Browse',
        	priority: 90,
        };
        mainMenuService.addMenuItem(menuItem);

        var journeyMenuItem = {
        	path: 'active',
        	title: 'Active',
        	icon: 'fa fa-tasks',
        	priority: 999
        };
        mainMenuService.addMenuItem(journeyMenuItem);


        $rootScope.$on('unauthorized', function (event, rejection) {
            $state.go('loginDialog');
        });

        $rootScope.$on('httpError', function (event, rejection) {
        	if (!(rejection.config.url.indexOf('api/notification') + 1)) {
        		notificationService.error({ title: 'HTTP error', description: rejection.status + ' — ' + rejection.statusText, extendedData: rejection.data });
          }
        });

        $rootScope.$on('loginStatusChanged', function (event, authContext) {
        	if (authContext.isAuthenticated) {
        		console.log('State - ' + $state.current.name);
        		if(!$state.current.name || $state.current.name == 'loginDialog')
        		{
        			homeMenuItem.action();
        		}
	        }
            else {
                $state.go('loginDialog');
            }
        });

        authService.fillAuthData();
    }
  ]
);

