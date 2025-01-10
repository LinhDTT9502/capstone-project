import { Button } from "@material-tailwind/react";
import React, { useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEye, faTimes, faVideo, faExclamationTriangle } from "@fortawesome/free-solid-svg-icons";

const ReturnRequestsPopup = ({ orderDetail }) => {
  const [isPopupVisible, setIsPopupVisible] = useState(false);

  const returnRequests =
    Array.isArray(orderDetail.returnRequests?.$values) &&
    orderDetail.returnRequests.$values;

  const handleOpenPopup = () => {
    setIsPopupVisible(true);
  };

  const handleClosePopup = () => {
    setIsPopupVisible(false);
  };

  return (
    <div>
      <Button
        color="blue"
        ripple={true}
        onClick={handleOpenPopup}
        className="flex items-center justify-center px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
      >
        <FontAwesomeIcon icon={faEye} className="mr-2" />
        Xem Danh Sách Yêu Cầu Hoàn Tiền
      </Button>

      {isPopupVisible && (
        <div
          className="fixed inset-0 z-50 overflow-y-auto"
          aria-labelledby="modal-title"
          role="dialog"
          aria-modal="true"
        >
          <div className="flex items-end justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
            <div
              className="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75"
              aria-hidden="true"
              onClick={handleClosePopup}
            ></div>

            <span
              className="hidden sm:inline-block sm:align-middle sm:h-screen"
              aria-hidden="true"
            >
              &#8203;
            </span>

            <div className="inline-block overflow-hidden text-left align-bottom transition-all transform bg-white rounded-lg shadow-xl sm:my-8 sm:align-middle ">
              <div className="px-4 pt-5 pb-4 bg-white sm:p-6 sm:pb-4 w-full">
                <div className="sm:flex sm:items-start">
                  <div className="mt-3 text-center sm:mt-0 sm:ml-4 sm:text-left">
                    <h3
                      className="text-lg font-medium leading-6 text-gray-900"
                      id="modal-title"
                    >
                      Danh Sách Yêu Cầu Hoàn Tiền
                    </h3>
                    <div className="mt-2">
                      <div className="overflow-y-auto max-h-[70vh] bg-gray-50 rounded-lg shadow-md">
                        {returnRequests && returnRequests.length > 0 ? (
                          <table className="w-full divide-y divide-gray-200">
                            <thead className="bg-gray-50">
                              <tr>
                                <th
                                  scope="col"
                                  className="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase"
                                >
                                  #
                                </th>
                                <th
                                  scope="col"
                                  className="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase"
                                >
                                  Mã sản phẩm
                                </th>
                                <th
                                  scope="col"
                                  className="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase"
                                >
                                  Lý do
                                </th>
                                <th
                                  scope="col"
                                  className="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase"
                                >
                                  Số tiền hoàn trả
                                </th>
                                <th
                                  scope="col"
                                  className="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase"
                                >
                                  Trạng thái
                                </th>
                                <th
                                  scope="col"
                                  className="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase"
                                >
                                  Video
                                </th>
                              </tr>
                            </thead>
                            <tbody className="bg-white divide-y divide-gray-200">
                              {returnRequests.map((item, index) => (
                                <tr
                                  key={index}
                                  className={
                                    index % 2 === 0 ? "bg-gray-50" : "bg-white"
                                  }
                                >
                                  <td className="px-6 py-4 text-sm text-gray-500 whitespace-nowrap">
                                    {index + 1}
                                  </td>
                                  <td className="px-6 py-4 text-sm text-gray-900 whitespace-nowrap">
                                    {item.productCode}
                                  </td>
                                  <td className="px-6 py-4 text-sm text-gray-500 whitespace-nowrap">
                                    {item.reason}
                                  </td>
                                  <td className="px-6 py-4 text-sm text-gray-500 whitespace-nowrap">
                                    {item.returnAmount.toLocaleString("vi-vn")}{" "}
                                    ₫
                                  </td>
                                  <td className="px-6 py-4 text-sm whitespace-nowrap">
                                    <span
                                      className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                                        item.status === "Approved"
                                          ? "bg-green-100 text-green-800"
                                          : item.status === "Pending"
                                          ? "bg-yellow-100 text-yellow-800"
                                          : "bg-red-100 text-red-800"
                                      }`}
                                    >
                                      {item.status}
                                    </span>
                                  </td>
                                  <td className="px-6 py-4 text-sm text-gray-500 whitespace-nowrap">
                                    {item.videoUrl ? (
                                      <video width="300" controls>
                                        <source
                                          src={item.videoUrl}
                                          type="video/mp4"
                                        />
                                        Your browser does not support the video
                                        tag.
                                      </video>
                                    ) : (
                                      <span className="text-gray-400">
                                        <FontAwesomeIcon
                                          icon={faExclamationTriangle}
                                          className="mr-1"
                                        />
                                        Không có
                                      </span>
                                    )}
                                  </td>
                                </tr>
                              ))}
                            </tbody>
                          </table>
                        ) : (
                          <p className="px-6 py-4 text-sm text-gray-500">
                            Không có yêu cầu hoàn tiền nào.
                          </p>
                        )}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              <div className="px-4 py-3 bg-gray-50 sm:px-6 sm:flex sm:flex-row-reverse">
                <button
                  type="button"
                  className="inline-flex justify-center w-full px-4 py-2 text-base font-medium text-white bg-red-600 border border-transparent rounded-md shadow-sm hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500 sm:ml-3 sm:w-auto sm:text-sm"
                  onClick={handleClosePopup}
                >
                  Đóng
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default ReturnRequestsPopup;

  