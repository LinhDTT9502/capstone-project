import React from "react";
import { NavLink, Outlet } from "react-router-dom";
import { useTranslation } from "react-i18next";

function ManageAccount() {
  const { t } = useTranslation();
  return (
    <div className="flex flex-col justify-center items-center md:flex-row py-12 ">
      <nav className="" >
        <ul className="space-y-4 font-poppins pr-10 ">
          <NavLink 
            to="/manage-account/profile" 
            className={({ isActive }) => isActive ? "text-orange-500" : "text-zinc-800"}
          >
            <li>{t("manage_account.my_profile")}</li>
          </NavLink>
          <NavLink 
            to="/manage-account/shipment" 
            className={({ isActive }) => isActive ? "text-orange-500" : "text-zinc-800"}
          >
            <li>{t("manage_account.address_book")}</li>
          </NavLink>
          {/* <li className="text-gray-500">My Payment Options</li>
          <li className="text-gray-500">My Returns</li>
          <li className="text-gray-500">My Cancellations</li>
          <li className="text-gray-500">My Wishlist</li> */}
        </ul>
      </nav>
      <div className="md:w-3/4 pl-10 border border-zinc-400 shadow-lg rounded-lg p-5">
        <Outlet />
      </div>
    </div>
  );
}

export default ManageAccount;
