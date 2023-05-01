
app.controller('UserViewCtrl', function ($scope, $http) {

	$scope.UserList = [];

	$scope.LoadUserViewList = function () {
		
		$http.post("/User/ViewUserList",
			{
			}
		).then(function (resp) {

			var mData = JSON.parse(resp.data);

			$scope.UserList = mData.Table;

		}, function (data) {
		});
	}

	$scope.LoadUserViewList();

	$scope.RedirectUserToEdit = function (vId) {
		window.location.href = "/User/Edit/" + vId;
	}
});
