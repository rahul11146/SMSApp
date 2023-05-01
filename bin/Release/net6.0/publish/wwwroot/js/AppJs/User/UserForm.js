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

	$scope.GetAllRoles();

	var mFloorId = $("#txtFloor1Id").val();

	if (mFloorId != undefined & mFloorId != null) {
		$scope.FlrId = mFloorId;
	}

	$scope.GetAllFloor = function () {

		$http.post("/ControllerMap/GetAllFloor",
			{
			}
		).then(function (resp) {

			var mData = JSON.parse(resp.data);

			$scope.FloorList = mData.Table;

		}, function (data) {
		});
	}
	
	$scope.GetAllFloor();

	var mDeptId = $("#txtDeptId").val();

	if (mDeptId != undefined & mDeptId != null) {
		$scope.DeptId = mDeptId;
	}

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

	$scope.RedirectUserToEdit = function (vId) {
		window.location.href = "/User/Edit/" + vId;
	}

	$scope.FormFloorList = [];

	$scope.AddFloor = function () {

		var mFloorId = $("#ddlFloor option:selected").val();
		var mFloorName = $("#ddlFloor option:selected").text();

		$scope.FormFloorList.push({
			FloorId: mFloorId,
			FloorName: mFloorName,
			IsType: "N"
		});
	}

});

function SaveUser() {

	$("#txtRoleId").val(_UserForm.RoleId);
	$("#txtFloor1Id").val(mFlrId);
	$("#txtDeptId").val(mDeptId);

	return true;
}

function OnFlrSel() {
	mFlrId = $("#ddlFloor option:selected").val().split(':')[1];
}

function OnDeptSel() {
	mDeptId = $("#ddlDept option:selected").val().split(':')[1];
}