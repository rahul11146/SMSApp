var _UserForm = null;
var mFlrId;
var mDeptId;

app.controller('UserFormCtrl', function ($scope, $http) {

	_UserForm = $scope;

	$scope.RoleList = [];
	$scope.FloorList = [];

	$scope.RoleId = "0";
	$scope.FlrId = "0";
	$scope.DeptId = "0";

	var mRoleId = $("#txtRoleId").val();

	if (mRoleId != undefined & mRoleId != null) {
		$scope.RoleId = mRoleId;
	}


	$scope.GetAllRoles = function () {

		$http.post("/User/GetAllRoles",
			{
			}
		).then(function (resp) {

			var mData = JSON.parse(resp.data);

			$scope.RoleList = mData.Table;

		}, function (data) {
		});
	}

	//$scope.GetAllRoles();

	var mFloorId = $("#txtFloor1Id").val();

	if (mFloorId != undefined & mFloorId != null) {
		$scope.FlrId = mFloorId;
	}

	$scope.GetAllFloor = function () {

		$http.post("/User/GetAllFloor",
			{
			}
		).then(function (resp) {

			var mData = JSON.parse(resp.data);

			$scope.FloorList = mData.Table;

		}, function (data) {
		});
	}

	//$scope.GetAllFloor();

	

	$scope.GetAllDepartment = function () {

		$http.post("/User/GetAllDepartment",
			{
			}
		).then(function (resp) {

			var mData = JSON.parse(resp.data);

			$scope.DeptList = mData.Table;

			var mDeptId = $("#txtDeptId").val();

			if (mDeptId != undefined && mDeptId != null && mDeptId != "") {
				$scope.DeptId = mDeptId;
			}

		}, function (data) {
		});
	}

	$scope.GetAllDepartment();

	$scope.RedirectUserToEdit = function (vId) {
		window.location.href = "/User/Edit/" + vId;
	}

	//$scope.FormFloorList = [];

	//var mFloorSelect = $("#txtFloorSelect").val();

	//if (mFloorSelect != undefined & mFloorSelect != null) {
	//	$scope.FormFloorList = JSON.parse(mFloorSelect);
	//}

	//$scope.AddFloor = function () {

	//	var mId = $scope.FormFloorList.length + 1;

	//	var mFloorId = $("#ddlFloor option:selected").val().split(':')[1];
	//	var mFloorName = $("#ddlFloor option:selected").text();

	//	$scope.FormFloorList.push({
	//		RowId: mId,
	//		FloorId: mFloorId,
	//		FloorName: mFloorName,
	//		Select: "",
	//		IsType: "N"
	//	});
	//}

	//$scope.RemoveFloor = function (vRowId) {

	//	for (var i = 0; i < $scope.FormFloorList.length; i++) {

	//		if ($scope.FormFloorList[i].RowId == vRowId) {
	//			$scope.FormFloorList[i].IsType = "D";
	//		}
	//	}

	//	alert(JSON.stringify($scope.FormFloorList));
	//}

});

function SaveUser() {

	//$("#txtRoleId").val(_UserForm.RoleId);
	//$("#txtFloor1Id").val(mFlrId);

	$("#txtDeptId").val(mDeptId);

	//if (_UserForm.FormFloorList != undefined && _UserForm.FormFloorList.length > 0) {
	//	$("#txtFloorSelect").val(JSON.stringify(_UserForm.FormFloorList));
	//}

	return true;
}

function OnFlrSel() {
	mFlrId = $("#ddlFloor option:selected").val().split(':')[1];
}

function OnDeptSel() {
	mDeptId = $("#ddlDept option:selected").val().split(':')[1];
}