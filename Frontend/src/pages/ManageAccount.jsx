import React from "react";
import { NavLink, Outlet } from "react-router-dom";
import { useTranslation } from "react-i18next";

function ManageAccount() {
  const { t } = useTranslation();
  return (
    <div className="px-20">
    <div className="flex flex-col md:flex-row py-12 gap-x-12"  >
      <nav className="pl-0 md:pl-0">
        <h1 className="font-semibold	" style={{ fontSize: "18px" }}>
          {t("manage_account.manage_my_account")}
        </h1>
        <ul className="space-y-4 font-poppins px-5 py-1">
          <NavLink
            to="/manage-account/profile"
            className={({ isActive }) =>
              isActive ? "text-orange-500" : "text-zinc-800"
            }
          >
            <li>{t("manage_account.my_profile")}</li>
          </NavLink>
          <NavLink 
            to="/manage-account/shipment"
            className={({ isActive }) =>
              isActive ? "text-orange-500" : "text-zinc-800"
            }
          >
            <li>{t("manage_account.address_book")}</li>
          </NavLink>
          <NavLink 
            to="/manage-account/change-password"
            className={({ isActive }) =>
              isActive ? "text-orange-500" : "text-zinc-800"
            }
          >
            <li>{t("manage_account.change_password")}</li>
          </NavLink>
          {/* <li className="text-gray-500">My Payment Options</li>
          <li className="text-gray-500">My Returns</li>
          <li className="text-gray-500">My Cancellations</li>
          <li className="text-gray-500">My Wishlist</li> */}
        </ul>

        <h1 className="font-semibold" style={{ fontSize: "18px" }}>
          {t("my_order")}
        </h1>
        <ul className="space-y-4 font-poppins px-5 py-1">
          <NavLink
            to="/manage-account/sale-order"
            className={({ 
              isActive
             }) =>
              isActive ? "text-orange-500" : "text-zinc-800"
            }
          >
            <li>Kiểm tra đơn hàng</li>
          </NavLink>

        </ul>
        <ul className="space-y-4 font-poppins px-5 py-1">
          <NavLink
            to="/manage-account/user-rental"
            className={({ 
              isActive
             }) =>
              isActive ? "text-orange-500" : "text-zinc-800"
            }
          >
            <li>Kiểm tra đơn thuê</li>
          </NavLink>
        </ul>
        
        <h1 className="font-semibold	" style={{ fontSize: "18px" }}>
          Gửi yêu cầu
        </h1>
        <ul className="space-y-4 font-poppins px-5 py-1">
          <NavLink
            to="/manage-account/refund-request"
            className={({ 
              isActive
             }) =>
              isActive ? "text-orange-500" : "text-zinc-800"
            }
          >
            <li>Trả hàng/Hoàn tiền</li>
          </NavLink>
        </ul>
      </nav>
      <div className="md:w-4/5	">
        <Outlet />
      </div>
    </div></div>
  );
}

export default ManageAccount;
