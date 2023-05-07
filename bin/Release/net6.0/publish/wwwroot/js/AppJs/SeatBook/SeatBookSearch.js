var _SeatBookSearch = null;

app.controller('SeatBookSearchCtrl', function ($scope, $http) {

	_SeatBookSearch = $scope;

	$scope.currentPage = 1;
	$scope.pageSize = 10;

	$scope.SeatBookSearchLst = [];

	$scope.SeatBookSearchList = function (mSrchType) {

		var mLastName = $(".txtLastName").val();
		var mFirstName = $(".txtFirstName").val();

		var mFloor = $('#ddlFloor').val();

		if (mFloor != null && mFloor != "") {
			mFloor = mFloor.toString();
		}

		var mDeptId = $('#ddlDept option:selected').val();

		if (mDeptId != null && mDeptId != "") {
			mDeptId = mDeptId.toString();
		}

		$http({
			method: "POST",
			url: "/SeatBook/SeatSearchList",
			params: {
				vLastName: mLastName,
				vFirstName: mFirstName,
				vFloorId: mFloor,
				vDeptId: mDeptId,
				vSrchType: mSrchType
			}
		}).then(function mySuccess(response) {

			var mData = JSON.parse(response.data);

			$scope.SeatBookSearchLst = mData.Table;

		}, function myError(response) {
			alert(response.statusText);
		});
	}

	$scope.ViewSeat = function (vFloorId, vFloorMapId) {

		window.location.href = "/Floor/EditMapFloor?id=" + vFloorId + "&FloorMapId=" + vFloorMapId + "&vType=Srch";
		return false;
	}

	$scope.FloorId = "0";

	$scope.GetAllFloor = function () {

		var mData = { 'vType': 'SeatSrch' };

		$http({
			method: "POST",
			url: "/UserAccess/GetAllFloorList",
			params: mData
		}).then(function mySuccess(response) {

			var mData = JSON.parse(response.data);

			$scope.FloorList = mData.Table;

		}, function myError(response) {
			alert(response.statusText);
		});
	}

	$scope.GetAllFloor();

	$scope.GetAllDepartment = function () {

		$http.post("/User/GetAllDepartment",
			{
			}
		).then(function (resp) {

			var mData = JSON.parse(resp.data);

			$scope.DeptList = mData.Table;


		}, function (data) {
		});
	}

	$scope.GetAllDepartment();

});

function OnUserSuccess() {
	swal({
		title: "SUCCESS!",
		text: "正常に完了しました。",
		type: "success",
		confirmButtonText: "OK"
	},
		function (isConfirm) {
			window.location.href = '/Floor/Index';
		});
}

function RedirectBack() {

	window.location.href = '/User/Index';

	return false;
}