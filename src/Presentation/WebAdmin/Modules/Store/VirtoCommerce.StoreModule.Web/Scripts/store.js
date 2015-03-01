﻿//Call this to register our module to main application
var moduleName = "virtoCommerce.storeModule";

if (AppDependencies != undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [
    'virtoCommerce.storeModule.blades',
    'virtoCommerce.storeModule.widgets'
])
.config(
  ['$stateProvider', function ($stateProvider) {
      $stateProvider
          .state('workspace.storeModule', {
              url: '/store',
              templateUrl: 'Modules/Store/VirtoCommerce.StoreModule.Web/Scripts/home.tpl.html',
              controller: [
                  '$scope', 'bladeNavigationService', function ($scope, bladeNavigationService) {
                      var blade = {
                          id: 'store',
                          title: 'Stores',
                          controller: 'storesListController',
                          template: 'Modules/Store/VirtoCommerce.StoreModule.Web/Scripts/blades/!stores-list.tpl.html',
                          isClosingDisabled: true
                      };
                      bladeNavigationService.showBlade(blade);
                  }
              ]
          });
  }]
)
.run(
  ['$rootScope', 'mainMenuService', 'widgetService', '$state', function ($rootScope, mainMenuService, widgetService, $state) {
      //Register module in main menu
      var menuItem = {
          path: 'browse/store',
          icon: 'fa fa-shopping-cart',
          title: 'Stores',
          priority: 110,
          action: function () { $state.go('workspace.storeModule'); },
          permission: 'storesMenuPermission'
      };
      mainMenuService.addMenuItem(menuItem);

      //Register widgets in store details
      widgetService.registerWidget({
          controller: 'storeLanguagesWidgetController',
          template: 'Modules/Store/VirtoCommerce.StoreModule.Web/Scripts/widgets/storeLanguagesWidget.tpl.html'
      }, 'storeDetail');
      widgetService.registerWidget({
          controller: 'storeCurrenciesWidgetController',
          template: 'Modules/Store/VirtoCommerce.StoreModule.Web/Scripts/widgets/storeCurrenciesWidget.tpl.html'
      }, 'storeDetail');
      widgetService.registerWidget({
          controller: 'storeAdvancedWidgetController',
          template: 'Modules/Store/VirtoCommerce.StoreModule.Web/Scripts/widgets/storeAdvancedWidget.tpl.html'
      }, 'storeDetail');
      widgetService.registerWidget({
          controller: 'storeSettingsWidgetController',
          template: 'Modules/Store/VirtoCommerce.StoreModule.Web/Scripts/widgets/storeSettingsWidget.tpl.html'
      }, 'storeDetail');
      widgetService.registerWidget({
          controller: 'storePaymentsWidgetController',
          template: 'Modules/Store/VirtoCommerce.StoreModule.Web/Scripts/widgets/storePaymentsWidget.tpl.html'
      }, 'storeDetail');
  }])
;