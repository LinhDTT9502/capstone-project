import React from "react";
import { Link, useLocation } from "react-router-dom";

const PolicyList = () => {
  const location = useLocation();

  const policies = [
    { path: '/complaints-handling', title: 'Chính sách xử lý khiếu nại' },
    { path: '/returns-refunds', title: 'Chính sách đổi trả và hoàn tiền' },
    { path: '/shipping', title: 'Chính sách vận chuyển' },
    { path: '/payment', title: 'Chính sách thanh toán' },
    { path: '/privacy', title: 'Chính sách bảo mật' },
    { path: '/membership', title: 'Chính sách thành viên' },
    { path: '/second-hand-rentals', title: 'Chính sách cho thuê đồ cũ' },
  ];

  return (
    <div className="w-1/4 bg-white p-6 rounded-lg shadow-md mr-8">
      <h2 className="text-2xl font-bold mb-6 text-gray-800 border-b pb-2">Danh Sách Chính Sách</h2>
      <ul className="space-y-2">
        {policies.map((policy) => (
          <li key={policy.path}>
            <Link
              to={policy.path}
              className={`block py-2 px-4 rounded transition-colors duration-200 ${
                location.pathname === policy.path
                  ? "bg-orange-500 text-white"
                  : "hover:bg-orange-100 text-gray-700 hover:text-orange-600"
              }`}
            >
              {policy.title}
            </Link>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default PolicyList;
