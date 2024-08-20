import React, { useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faMagnifyingGlass,
  faBell,
  faCaretDown,
} from "@fortawesome/free-solid-svg-icons";
import { Menu, MenuHandler, MenuList, MenuItem, Button } from "@material-tailwind/react";
import { useTranslation } from "react-i18next";
import Logout from "../components/Auth/Logout";
import { useSelector } from "react-redux";
import { selectUser } from "../redux/slices/authSlice";


function HeaderStaff() {
  const { t } = useTranslation();
  const [open, setOpen] = useState(false);
  const user = useSelector(selectUser)

  const handleMenuToggle = () => {
    setOpen(!open);
  };

  return (
    <div className="justify-between flex items-center py-5 space-x-4 border-2">
      <div className="pl-10">
        <img
          src="/assets/images/Logo.png"
          alt="2Sport"
          className="object-scale-down w-20 "
        />
      </div>
      <div className="flex">
        <div className="flex items-center pr-5">
          <FontAwesomeIcon icon={faMagnifyingGlass} className="text-xl" />
        </div>
        <div className="flex items-center pr-5">
          <FontAwesomeIcon icon={faBell} className="text-xl" />
        </div>
        <div className="w-fit rounded font-bold pr-5 bg-transparent">
          <Menu open={open} handler={setOpen}>
            <MenuHandler>
              <Button
                className="w-fit h-10 text-black bg-transparent flex  items-center justify-center"
                onClick={handleMenuToggle}
              >
                <p className="pr-2 ">{user.role}:</p>
                <p className="text-orange-500">{user.FullName}</p>

                <FontAwesomeIcon icon={faCaretDown} className="ml-2" />
              </Button>
            </MenuHandler>
            <MenuList>
              <MenuItem >
                <Logout />
              </MenuItem>
            </MenuList>
          </Menu>
        </div>
      </div>

    </div>
  );
}

export default HeaderStaff;

