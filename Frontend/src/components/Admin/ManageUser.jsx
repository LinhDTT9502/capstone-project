import { useState, useEffect } from "react";
import {
  Input,
  Button,
  Card,
  Typography,
  Checkbox,
  CardBody,
  Breadcrumbs,
  Dialog,
  DialogHeader,
  DialogBody,
  DialogFooter,
} from "@material-tailwind/react";
import { createUser, updateUser, getUserDetails, changeUserStatus } from "../../api/apiUser";
import { fetchAllUsers } from "../../services/ManageUserService";
import { useTranslation } from "react-i18next";
import HeaderStaff from "../../layouts/HeaderStaff";
import SidebarStaff from "../../layouts/SidebarStaff";
import EditIcon from "@mui/icons-material/Edit";
import VisibilityIcon from "@mui/icons-material/Visibility";
import { toast, ToastContainer } from "react-toastify";
import 'react-toastify/dist/ReactToastify.css';

export default function ManageUser() {
  const { t } = useTranslation();
  const [users, setUsers] = useState([]);
  const [selectedRowKeys, setSelectedRowKeys] = useState([]);
  const [searchQuery, setSearchQuery] = useState("");
  const [isModalOpen, setModalOpen] = useState(false);
  const [isEditModalOpen, setEditModalOpen] = useState(false);
  const [viewUserModalOpen, setViewUserModalOpen] = useState(false);
  const [viewUserData, setViewUserData] = useState(null);
  const [editUserData, setEditUserData] = useState(null);
  const [newUserData, setNewUserData] = useState({
    username: "",
    password: "",
    fullName: "",
    phone: "",
    email: "",
    gender: "",
    birthDate: "",
    roleId: "",
    isActive: true,
  });

  // Pagination state
  const [currentPage, setCurrentPage] = useState(1);
  const usersPerPage = 7;

  useEffect(() => {
    const fetchData = async () => {
      try {
        const usersData = await fetchAllUsers();
        setUsers(usersData);
      } catch (error) {
        console.error(error);
        setUsers([]);
        toast.error(t("manage_user.fetch_error"));
      }
    };

    fetchData();
  }, [t]);

  // Calculate paginated users
  const indexOfLastUser = currentPage * usersPerPage;
  const indexOfFirstUser = indexOfLastUser - usersPerPage;
  const currentUsers = users.slice(indexOfFirstUser, indexOfLastUser);

  // Go to the next page
  const handleNextPage = () => {
    if (currentPage < Math.ceil(users.length / usersPerPage)) {
      setCurrentPage(currentPage + 1);
    }
  };

  // Go to the previous page
  const handlePrevPage = () => {
    if (currentPage > 1) {
      setCurrentPage(currentPage - 1);
    }
  };

  // Validation function to check email format
  const isValidEmail = (email) => {
    const emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
    return emailPattern.test(email);
  };

  // Validation function to check phone number format
  const isValidPhoneNumber = (phone) => {
    const phonePattern = /^[0-9]{10}$/;
    return phonePattern.test(phone);
  };

  // Validation function to check if birthdate is in the past
  const isValidBirthdate = (birthDate) => {
    const selectedDate = new Date(birthDate);
    const today = new Date();
    return selectedDate <= today;
  };

  // Validation function for gender
  const isValidGender = (gender) => {
    return ["male", "female", "other"].includes(gender);
  };

  // Handle input change for both create and edit forms
  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setNewUserData((prevData) => ({
      ...prevData,
      [name]: value,
    }));
  };

  const handleEditInputChange = (e) => {
    const { name, value } = e.target;
    setEditUserData((prevData) => ({
      ...prevData,
      [name]: value,
    }));
  };

  // Handle user creation with validation
  const handleCreateUser = async () => {
    const { username, password, fullName, phone, email, gender, birthDate, roleId } = newUserData;

    if (!username || !password || !fullName || !phone || !email || !gender || !birthDate || !roleId) {
      toast.error(t("manage_user.missing_fields"));
      return;
    }

    if (!isValidEmail(email)) {
      toast.error(t("manage_user.invalid_email"));
      return;
    }

    if (!isValidPhoneNumber(phone)) {
      toast.error(t("manage_user.invalid_phone"));
      return;
    }

    if (!isValidGender(gender)) {
      toast.error(t("manage_user.invalid_gender"));
      return;
    }

    if (!isValidBirthdate(birthDate)) {
      toast.error(t("manage_user.invalid_birthdate"));
      return;
    }

    try {
      await createUser(newUserData);
      const updatedUsers = await fetchAllUsers();
      setUsers(updatedUsers);
      setModalOpen(false);
      setNewUserData({
        username: "",
        password: "",
        fullName: "",
        phone: "",
        email: "",
        gender: "",
        birthDate: "",
        roleId: "",
        isActive: true,
      });
      toast.success(t("manage_user.create_success"));
    } catch (error) {
      toast.error(`${t("manage_user.create_error")}`);
    }
  };

  // Handle updating user information
  const handleUpdateUser = async () => {
    const { username, fullName, phone, email, gender, birthDate, roleId } = editUserData;

    if (!username || !fullName || !phone || !email || !gender || !birthDate || !roleId) {
      toast.error(t("manage_user.missing_fields"));
      return;
    }

    if (!isValidEmail(email)) {
      toast.error(t("manage_user.invalid_email"));
      return;
    }

    if (!isValidPhoneNumber(phone)) {
      toast.error(t("manage_user.invalid_phone"));
      return;
    }

    if (!isValidGender(gender)) {
      toast.error(t("manage_user.invalid_gender"));
      return;
    }

    if (!isValidBirthdate(birthDate)) {
      toast.error(t("manage_user.invalid_birthdate"));
      return;
    }

    try {
      await updateUser(editUserData.id, editUserData);
      const updatedUsers = await fetchAllUsers();
      setUsers(updatedUsers);
      setEditModalOpen(false);
      toast.success(t("manage_user.update_success"));
    } catch (error) {
      toast.error(`${t("manage_user.update_error")}`);
    }
  };

  // Handle changing user status
  const handleChangeStatus = async (userId, currentStatus) => {
    const newStatus = !currentStatus;

    try {
      await changeUserStatus(userId, { isActive: newStatus });
      const updatedUsers = await fetchAllUsers();
      setUsers(updatedUsers);
      toast.success(`Status changed to ${newStatus ? "Active" : "Inactive"}`);
    } catch (error) {
      toast.error(`${t("manage_user.update_error")}`);
    }
  };

  // Handle viewing user details
  const handleViewUser = async (userId) => {
    try {
      const response = await getUserDetails(userId);
      const userData = response.data?.user?.data;
      if (userData) {
        setViewUserData(userData);
        setViewUserModalOpen(true);
      } else {
        toast.error(t("manage_user.fetch_error"));
      }
    } catch (error) {
      console.error("Error fetching user details:", error);
      toast.error(t("manage_user.fetch_error"));
    }
  };
  

  const onSelectChange = (selectedKey) => {
    setSelectedRowKeys((prevSelectedRowKeys) =>
      prevSelectedRowKeys.includes(selectedKey)
        ? prevSelectedRowKeys.filter((key) => key !== selectedKey)
        : [...prevSelectedRowKeys, selectedKey]
    );
  };

  const getRoleName = (roleId) => {
    switch (roleId) {
      case 2:
        return t("manage_user.manager");
      case 3:
        return t("manage_user.employee");
      case 4:
        return t("manage_user.customer");
      case 5:
        return t("manage_user.owner");
      default:
        return t("manage_user.unknown_role");
    }
  };

  const getStatusButtonStyle = (isActive) => {
    return isActive ? "bg-blue-500 text-white" : "bg-gray-500 text-white";
  };

  const filteredUsers = users.filter((user) => {
    const query = searchQuery.toLowerCase();
    return (
      user.fullName.toLowerCase().includes(query) ||
      user.userName.toLowerCase().includes(query)
    );
  });

  return (
    <>
      <ToastContainer />
      <HeaderStaff />
      <div className="flex h-full">
        <SidebarStaff />
        <div className="flex-grow border-l-2">
          <h2 className="text-2xl font-bold mx-10 mt-4">
            {t("dashboard.customer_account")}
          </h2>
          <div className="flex justify-between items-center mx-10 my-4">
            <Breadcrumbs className="flex-grow">
              <a href="#" className="opacity-60">
                {t("dashboard.home")}
              </a>
              <a href="#">{t("dashboard.customer_account")}</a>
            </Breadcrumbs>
            <Button onClick={() => setModalOpen(true)} className="mt-4">
              {t("manage_user.create_user")}
            </Button>
          </div>

          <Card className="h-full w-[95.7%] mx-10 my-10">
            <CardBody className="overflow-scroll px-0">
              <table className="w-full min-w-max table-auto text-left">
                <thead>
                  <tr>
                    <th className="border-y border-blue-gray-100 bg-blue-gray-50/50 p-4">
                      <Checkbox
                        color="blue"
                        onChange={(e) => {
                          if (e.target.checked) {
                            setSelectedRowKeys(users.map((row) => row.id));
                          } else {
                            setSelectedRowKeys([]);
                          }
                        }}
                        checked={selectedRowKeys.length === users.length}
                        indeterminate={
                          selectedRowKeys.length > 0 &&
                          selectedRowKeys.length < users.length
                        }
                      />
                    </th>
                    <th className="border-y border-blue-gray-100 bg-blue-gray-50/50 p-4">
                      <Typography
                        variant="large"
                        color="blue-gray"
                        className="font-normal leading-none opacity-70"
                      >
                        {t("manage_user.username")}
                      </Typography>
                    </th>
                    <th className="border-y border-blue-gray-100 bg-blue-gray-50/50 p-4">
                      <Typography
                        variant="large"
                        color="blue-gray"
                        className="font-normal leading-none opacity-70"
                      >
                        {t("manage_user.email")}
                      </Typography>
                    </th>
                    <th className="border-y border-blue-gray-100 bg-blue-gray-50/50 p-4">
                      <Typography
                        variant="large"
                        color="blue-gray"
                        className="font-normal leading-none opacity-70"
                      >
                        {t("manage_user.role")}
                      </Typography>
                    </th>
                    <th className="border-y border-blue-gray-100 bg-blue-gray-50/50 p-4">
                      <Typography
                        variant="large"
                        color="blue-gray"
                        className="font-normal leading-none opacity-70"
                      >
                        {t("manage_user.actions")}
                      </Typography>
                    </th>
                    <th className="border-y border-blue-gray-100 bg-blue-gray-50/50 p-4">
                      <Typography
                        variant="large"
                        color="blue-gray"
                        className="font-normal leading-none opacity-70"
                      >
                        {t("manage_user.status")}
                      </Typography>
                    </th>
                  </tr>
                </thead>
                <tbody>
                  {currentUsers.map((user, index) => {
                    const isLast = index === users.length - 1;
                    const classes = isLast
                      ? "p-4"
                      : "p-4 border-b border-blue-gray-50";
                    const isSelected = selectedRowKeys.includes(user.id);

                    return (
                      <tr
                        key={user.id}
                        className={isSelected ? "bg-blue-100" : ""}
                      >
                        <td className={classes}>
                          <Checkbox
                            color="blue"
                            checked={isSelected}
                            onChange={() => onSelectChange(user.id)}
                          />
                        </td>
                        <td className={classes}>
                          <Typography
                            variant="small"
                            color="blue-gray"
                            className="font-normal"
                          >
                            {user.userName}
                          </Typography>
                        </td>
                        <td className={classes}>
                          <Typography
                            variant="small"
                            color="blue-gray"
                            className="font-normal"
                          >
                            {user.email}
                          </Typography>
                        </td>
                        <td className={classes}>
                          <Typography
                            variant="small"
                            color="blue-gray"
                            className="font-normal"
                          >
                            {getRoleName(user.roleId)}
                          </Typography>
                        </td>
                        <td className={classes}>
                          <div className="flex space-x-2">
                            <Button
                              variant="text"
                              className="hover:underline"
                              onClick={() => handleEditUser(user.id)}
                              title="Edit"
                            >
                              <EditIcon />
                              <span className="sr-only">Edit</span>
                            </Button>
                            <Button
                              variant="text"
                              className="hover:underline"
                              onClick={() => handleViewUser(user.id)}
                              title="View Information"
                            >
                              <VisibilityIcon />
                              <span className="sr-only">View Information</span>
                            </Button>
                          </div>
                        </td>
                        <td className={classes}>
                          <Button
                            className={`${getStatusButtonStyle(user.isActive)} px-4 py-2`}
                            onClick={() => handleChangeStatus(user.id, user.isActive)}
                          >
                            {user.isActive ? t("manage_user.active") : t("manage_user.inactive")}
                          </Button>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </CardBody>

            {/* Pagination controls */}
            <div className="flex justify-between mx-10 my-4">
              <Button onClick={handlePrevPage} disabled={currentPage === 1}>
                Prev
              </Button>
              <Typography variant="small" className="font-normal">
                Page {currentPage} of {Math.ceil(users.length / usersPerPage)}
              </Typography>
              <Button onClick={handleNextPage} disabled={currentPage === Math.ceil(users.length / usersPerPage)}>
                Next
              </Button>
            </div>
          </Card>
        </div>
      </div>

      {/* View User Modal */}
      <Dialog open={viewUserModalOpen} onClose={() => setViewUserModalOpen(false)} size="lg">
  <DialogHeader>{t("manage_user.view_user_details")}</DialogHeader>
  <DialogBody className="grid grid-cols-2 gap-4">
    <div>
      <Typography variant="small" className="font-bold">
        {t("manage_user.username")}:
      </Typography>
      <Typography variant="small" className="mb-4">
        {viewUserData?.userName || t("manage_user.unknown")}  {/* Corrected field */}
      </Typography>

      <Typography variant="small" className="font-bold">
        {t("manage_user.fullname")}:
      </Typography>
      <Typography variant="small" className="mb-4">
        {viewUserData?.fullName || t("manage_user.unknown")}
      </Typography>

      <Typography variant="small" className="font-bold">
        {t("manage_user.email")}:
      </Typography>
      <Typography variant="small" className="mb-4">
        {viewUserData?.email || t("manage_user.unknown")}
      </Typography>

      <Typography variant="small" className="font-bold">
        {t("manage_user.gender")}:
      </Typography>
      <Typography variant="small" className="mb-4">
        {viewUserData?.gender || t("manage_user.unknown")}
      </Typography>
    </div>
    <div>
      <Typography variant="small" className="font-bold">
        {t("manage_user.phone")}:
      </Typography>
      <Typography variant="small" className="mb-4">
        {viewUserData?.phone || t("manage_user.unknown")}
      </Typography>

      <Typography variant="small" className="font-bold">
        {t("manage_user.birthdate")}:
      </Typography>
      <Typography variant="small" className="mb-4">
        {viewUserData?.birthDate ? new Date(viewUserData.birthDate).toLocaleDateString() : t("manage_user.unknown")}
      </Typography>

      <Typography variant="small" className="font-bold">
        {t("manage_user.role")}:
      </Typography>
      <Typography variant="small" className="mb-4">
        {getRoleName(viewUserData?.roleId)}
      </Typography>

      <Typography variant="small" className="font-bold">
        {t("manage_user.status")}:
      </Typography>
      <Typography variant="small" className="mb-4">
        {viewUserData?.isActive ? t("manage_user.active") : t("manage_user.inactive")}
      </Typography>
    </div>
  </DialogBody>
  <DialogFooter>
    <Button variant="text" onClick={() => setViewUserModalOpen(false)} className="mr-1">
      {t("manage_user.close")}
    </Button>
  </DialogFooter>
</Dialog>

    </>
  );
}
