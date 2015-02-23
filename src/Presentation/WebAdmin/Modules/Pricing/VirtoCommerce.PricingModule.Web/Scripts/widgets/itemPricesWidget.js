﻿angular.module('virtoCommerce.pricingModule.widget.itemPricesWidget', [
    'virtoCommerce.pricingModule.blades.item',
    'virtoCommerce.pricingModule.resources.pricing'
])
.controller('itemPricesWidgetController', ['$scope', 'bladeNavigationService', 'prices', function ($scope, bladeNavigationService, prices) {
    $scope.currentBlade = $scope.widget.blade;
    $scope.priceRange = '...';

    $scope.openBlade = function () {
        if ($scope.priceRange !== '...') {
            var newBlade = {
                id: "itemPricelists",
                itemId: $scope.currentBlade.itemId,
                title: $scope.currentBlade.title,
                subtitle: 'Select Price list to manage prices',
                controller: 'itemPricelistsListController',
                template: 'Modules/Pricing/VirtoCommerce.PricingModule.Web/Scripts/blades/item/item-pricelists-list.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, $scope.currentBlade);
        }
    };

    prices.query({ id: $scope.currentBlade.itemId }, function (data) {
        // find the most popular currency and min/max prices in it.
        var prices = _.flatten(_.pluck(data, 'prices'), true);
        if (prices.length) {
            prices = _.groupBy(prices, 'currency');
            prices = _.max(_.values(prices), function (x) { return x.length; });
            var allPrices = _.union(_.pluck(prices, 'list'), _.pluck(prices, 'sale'));
            var minprice = _.min(allPrices);
            var maxprice = _.max(allPrices);
            var currency = prices.length ? ' ' + prices[0].currency : '';
            $scope.priceRange = (minprice == maxprice ? minprice : minprice + '-' + maxprice) + currency;
        } else {
            $scope.priceRange = 'NO';
        }
    });
}]);
