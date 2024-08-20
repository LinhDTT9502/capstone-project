import { useEffect, useState } from "react";
import {
  Card,
  Breadcrumbs,
  CardBody,
  Typography,
  Avatar,
  Checkbox,
} from "@material-tailwind/react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faEllipsisVertical,
  faArrowUp,
  faCalendar,
} from "@fortawesome/free-solid-svg-icons";
import { fetchAllUsers } from "../../services/ManageUserService";
import { useTranslation } from "react-i18next";

export default function ManageUser() {
  const { t } = useTranslation();
  const [users, setUsers] = useState([]);
  const [selectedRowKeys, setSelectedRowKeys] = useState([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const usersData = await fetchAllUsers();
        setUsers(usersData);
        console.log(usersData);
      } catch (error) {
        console.log(error);
        setUsers([]);
      }
    };

    fetchData();
  }, []);

  const onSelectChange = (selectedKey) => {
    setSelectedRowKeys((prevSelectedRowKeys) =>
      prevSelectedRowKeys.includes(selectedKey)
        ? prevSelectedRowKeys.filter((key) => key !== selectedKey)
        : [...prevSelectedRowKeys, selectedKey]
    );
  };

  return (
    <>
      <Card className="h-full w-[95.7%] mx-10 my-10">
        <Typography variant="h6" color="black" className="mx-10 mt-4 text-2xl">
          {t("manage_user.user")}
        </Typography>

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
                    {t("manage_user.fullname")}
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
                    {t("manage_user.rolename")}
                  </Typography>
                </th>
                <th className="border-y border-blue-gray-100 bg-blue-gray-50/50 p-4">
                  <Typography
                    variant="large"
                    color="blue-gray"
                    className="font-normal leading-none opacity-70"
                  >
                    {t("manage_user.gender")}
                  </Typography>
                </th>
                <th className="border-y border-blue-gray-100 bg-blue-gray-50/50 p-4">
                  <Typography
                    variant="large"
                    color="blue-gray"
                    className="font-normal leading-none opacity-70"
                  >
                    {t("manage_user.phone")}
                  </Typography>
                </th>
                <th className="border-y border-blue-gray-100 bg-blue-gray-50/50 p-4">
                  <Typography
                    variant="large"
                    color="blue-gray"
                    className="font-normal leading-none opacity-70"
                  >
                    {t("manage_user.birthdate")}
                  </Typography>
                </th>
                <th className="border-y border-blue-gray-100 bg-blue-gray-50/50 p-4">
                  <Typography
                    variant="large"
                    color="blue-gray"
                    className="font-normal leading-none opacity-70"
                  >
                    {t("manage_user.createddate")}
                  </Typography>
                </th>
                <th className="border-y border-blue-gray-100 bg-blue-gray-50/50 p-4">
                  <Typography
                    variant="large"
                    color="blue-gray"
                    className="font-normal leading-none opacity-70"
                  >
                    {t("manage_user.lastupdate")}
                  </Typography>
                </th>
              </tr>
            </thead>
            <tbody>
              {users.map((user, index) => {
                const isLast = index === users.length - 1;
                const classes = isLast
                  ? "p-4"
                  : "p-4 border-b border-blue-gray-50";
                const isSelected = selectedRowKeys.includes(user.id);

                return (
                  <tr key={user.id} className={isSelected ? "bg-blue-100" : ""}>
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
                        {user.fullName}
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
                      <div className="flex items-center">
                        {/* <Avatar
                          size="sm"
                          src={user.shipmentDetail.user.avatarUrl}
                          className="rounded-full mr-2 w-8 h-8"
                          alt={user.shipmentDetail.fullName}
                        /> */}
                        <Typography
                          variant="small"
                          color="blue-gray"
                          className="font-normal"
                        >
                          {user.roleName}
                        </Typography>
                      </div>
                    </td>
                    <td className={classes}>
                      <div className="flex items-center">
                        {/* <span
                          className={`inline-block w-2 h-2 mr-2 rounded-full ${
                            user.status === "Delivered"
                              ? "bg-green-500"
                              : user.status === "Cancelled"
                              ? "bg-red-500"
                              : "bg-gray-500"
                          }`}
                        ></span> */}
                        <Typography
                          variant="small"
                          color="blue-gray"
                          className="font-normal"
                        >
                          {user.gender}
                        </Typography>
                      </div>
                    </td>
                    <td className={classes}>
                      <Typography
                        variant="small"
                        color="blue-gray"
                        className="font-normal"
                      >
                        {user.phone}
                      </Typography>
                    </td>
                    <td className={classes}>
                      <Typography
                        variant="small"
                        color="blue-gray"
                        className="font-normal"
                      >
                        {new Date(user.birthDate).toLocaleDateString()}
                      </Typography>
                    </td>
                    <td className={classes}>
                      <Typography
                        variant="small"
                        color="blue-gray"
                        className="font-normal"
                      >
                        {new Date(user.createdDate).toLocaleDateString()}
                      </Typography>
                    </td>
                    <td className={classes}>
                      <Typography
                        variant="small"
                        color="blue-gray"
                        className="font-normal"
                      >
                        {new Date(user.lastUpdate).toLocaleDateString()}
                      </Typography>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </CardBody>
      </Card>
    </>
  );
}
