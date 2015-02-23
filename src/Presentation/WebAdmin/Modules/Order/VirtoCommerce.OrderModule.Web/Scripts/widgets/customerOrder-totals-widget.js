﻿angular.module('virtoCommerce.orderModule.widgets')
.controller('customerOrderTotalsWidgetController', ['$scope', 'calculateTotalsService', 'bladeNavigationService', function ($scope, calculateTotalsService, bladeNavigationService) {
	$scope.currentBlade = $scope.widget.blade;
	$scope.customerOrder = {};

	$scope.$watch('widget.blade.currentEntity', function (customerOrder) {
		if (customerOrder) {
			calculateTotalsService.recalculateTotals(customerOrder);
			$scope.customerOrder = customerOrder;
		}
	});
}]);
