import React, { useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEye, faTimes, faVideo, faExclamationTriangle } from "@fortawesome/free-solid-svg-icons";
import {
  Dialog,
  DialogHeader,
  DialogBody,
  DialogFooter,
  Button,
  Chip,
  Icon,
} from "@material-tailwind/react";

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
    <>
      <Button
        size="sm"
        color="blue"
        ripple={true}
        onClick={handleOpenPopup}
        className="  text-rose-700 bg-white border border-rose-700 rounded-md hover:bg-rose-200"
      >
        Xem Yêu Cầu Hoàn trả
      </Button>

      {isPopupVisible && (
        <Dialog open={isPopupVisible} handler={handleClosePopup} size="lg">
          <DialogHeader>Danh Sách Yêu Cầu Hoàn Tiền</DialogHeader>
          <DialogBody>
            {returnRequests && returnRequests.length > 0 ? (
              <div className="overflow-y-auto max-h-[70vh] bg-gray-50 rounded-lg shadow-md">
                <table className="w-full divide-y divide-gray-200">
                  <thead className="bg-gray-100">
                    <tr>
                      <th className="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase">
                        #
                      </th>
                      <th className="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase">
                        Mã sản phẩm
                      </th>
                      <th className="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase">
                        Lý do
                      </th>
                      <th className="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase">
                        Số tiền hoàn trả
                      </th>
                      <th className="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase">
                        Trạng thái
                      </th>
                      <th className="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase">
                        Video
                      </th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-gray-200">
                    {returnRequests.map((item, index) => (
                      <tr
                        key={index}
                        className={index % 2 === 0 ? "bg-gray-50" : "bg-white"}
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
                          {item.returnAmount.toLocaleString("vi-vn")} ₫
                        </td>
                        <td className="px-6 py-4 text-sm whitespace-nowrap">
                          {console.log(item)}
                          <Chip
                            color={
                              item.status === "APPROVED"
                                ? "green"
                                : item.status === "PENDING"
                                ? "yellow"
                                : "red"
                            }
                            value={item.status}
                            variant="filled"
                            className="text-xs font-semibold"
                          />
                        </td>
                        <td className="px-6 py-4 text-sm text-gray-500 whitespace-nowrap">
                          {item.videoUrl ? (
                            <div className="w-full max-w-[150px] h-auto overflow-hidden flex justify-center items-center group">
                              <video
                                width="200"
                                className="max-w-full max-h-[100px] object-cover transition-transform duration-300 ease-in-out"
                                controls
                              >
                                <source src={item.videoUrl} type="video/mp4" />
                                Trình duyệt không hỗ trợ video.
                              </video>
                            </div>
                          ) : (
                            <span className="text-gray-400">
                              Không có video
                            </span>
                          )}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            ) : (
              <p className="px-6 py-4 text-sm text-gray-500">
                Không có yêu cầu trả hàng/ hoàn tiền nào.
              </p>
            )}
          </DialogBody>
          <DialogFooter>
            <Button
              color="red"
              onClick={handleClosePopup}
              ripple="dark"
              className="w-full sm:w-auto"
            >
              Đóng
            </Button>
          </DialogFooter>
        </Dialog>
      )}
    </>
  );
};

export default ReturnRequestsPopup;

  