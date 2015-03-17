﻿angular.module('virtoCommerce.orderModule.wizards')
.controller('newOperationWizardController', ['$scope', 'bladeNavigationService', 'dialogService', 'order_res_customerOrders', function ($scope, bladeNavigationService, dialogService, order_res_customerOrders) {

	$scope.blade.isLoading = false;
	var shipmentOperation =
		{
			name: 'Shipment',
			descr: 'Add new shipment',
			action: function () {

				order_res_customerOrders.getNewShipment({ id: $scope.blade.customerOrder.id }, function (result) {

					bladeNavigationService.closeBlade($scope.blade);

					$scope.blade.customerOrder.shipments.push(result);
					$scope.blade.customerOrder.childrenOperations.push(result);

					var newBlade = {
						id: 'operationDetail',
						title: 'Shipment #' + result.number,
						subtitle: 'Edit shipment details',
						isNew: true,
						customerOrder: $scope.blade.customerOrder,
						currentEntity: result,
						isClosingDisabled: false,
						controller: 'operationDetailController',
						template: 'Modules/$(VirtoCommerce.Orders)/Scripts/blades/shipment-detail.tpl.html'
					};
				
					bladeNavigationService.showBlade(newBlade);
				});
			}
		};

	var paymentOperation =
	{
		name: 'Incoming payment',
		descr: 'Add new incoming payment',
		action: function () {

			order_res_customerOrders.getNewPayment({ id: $scope.blade.customerOrder.id }, function (result) {

				bladeNavigationService.closeBlade($scope.blade);

				$scope.blade.customerOrder.inPayments.push(result);
				$scope.blade.customerOrder.childrenOperations.push(result);

				var newBlade = {
					id: 'operationDetail',
					title: 'Incoming payment #' + result.number,
					subtitle: 'Edit payment details and related documents',
					customerOrder: $scope.blade.customerOrder,
					currentEntity: result,
					isNew: true,
					controller: 'operationDetailController',
					template: 'Modules/$(VirtoCommerce.Orders)/Scripts/blades/payment-detail.tpl.html'
				};
			
				bladeNavigationService.showBlade(newBlade);
			});
		}

	};


	$scope.availOperationsMap = {
		'customerorder': [shipmentOperation, paymentOperation],
		'shipment': [],
		'payment': []
	};

	$scope.getAvailOperations = function () {
		return $scope.availOperationsMap[$scope.blade.currentEntity.operationType.toLowerCase()];
	};



}]);


