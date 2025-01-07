import React from "react";
import { NavLink, Outlet } from "react-router-dom";
import { useTranslation } from "react-i18next";

function GuestManagement() {
  const { t } = useTranslation();
  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-100 to-orange-50">
      <div className="container mx-auto px-4 py-8 md:px-8 lg:px-12">
        <div className="flex flex-col md:flex-row gap-8">
          {/* Sidebar */}
          <nav className="md:w-1/4 bg-white shadow-lg rounded-lg p-6 md:sticky md:top-8 h-fit">
            <h1 className="font-semibold text-xl mb-4 text-gray-800 border-b pb-2">
              {t("my_order")}
            </h1>
            <ul className="space-y-4 font-poppins">
              <NavLink
                to="/guest/guest-sale-order"
                className={({ isActive }) =>
                  `block py-2 px-4 rounded transition-colors ${
                    isActive
                      ? "bg-orange-100 text-orange-600"
                      : "text-gray-600 hover:bg-gray-100"
                  }`
                }
              >
                <li>Kiểm tra đơn mua</li>
              </NavLink>
              <NavLink
                to="/guest/guest-rent-order"
                className={({ isActive }) =>
                  `block py-2 px-4 rounded transition-colors ${
                    isActive
                      ? "bg-orange-100 text-orange-600"
                      : "text-gray-600 hover:bg-gray-100"
                  }`
                }
              >
                <li>Kiểm tra đơn thuê</li>
              </NavLink>
            </ul>
          </nav>

          {/* Content Area */}
          <div className="flex-1 bg-white shadow-lg rounded-lg p-6 h-max">
            <Outlet />
          </div>
        </div>
      </div>
    </div>
  );
}

export default GuestManagement;
