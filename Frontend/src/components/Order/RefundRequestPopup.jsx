import React, { useEffect, useState } from "react";
import {
  Dialog,
  DialogHeader,
  DialogBody,
  DialogFooter,
  Button,
  Chip,
  Icon,
} from "@material-tailwind/react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCheckCircle, faHourglass, faTimesCircle } from "@fortawesome/free-solid-svg-icons";
import { fetchBranchs } from "../../services/branchService";

const RefundRequestPopup = ({ refundRequests }) => {
  const [open, setOpen] = useState(false);
  const [branches, setBranches] = useState([]);
  const toggleDialog = () => setOpen(!open);

const getBranchName = (branchId) => {
  const branch = branches.find((b) => b.id === branchId);
  return branch ? branch.branchName : "Unknown Branch";
};

  const statusColors = {
    Pending: "yellow",
    Approved: "green",
    Rejected: "red",
  };

  useEffect(() => {
    const loadBranches = async () => {
      try {
        const data = await fetchBranchs(); // Replace this with your API call
        setBranches(data);
      } catch (error) {
        console.error('Error fetching branch data:', error);
      }
    };
    loadBranches();
  }, []);

  return (
    <>
      <Button
        size="sm"
        onClick={toggleDialog}
        ripple={true}
        className=" text-blue-700 bg-white border border-blue-700 rounded-md hover:bg-blue-200"
      >
        Xem yêu cầu hoàn tiền
      </Button>

      <Dialog open={open} handler={toggleDialog} size="xl">
        <DialogHeader className="text-lg font-bold text-orange-500 uppercase">
          Danh sách Yêu cầu hoàn tiền
        </DialogHeader>
        <DialogBody divider>
          {refundRequests?.$values && refundRequests.$values.length > 0 ? (
            <div className="overflow-x-auto">
              <table className="table-auto w-full text-left border-collapse border border-gray-200">
                <thead>
                  <tr className="bg-gray-100 text-gray-700 uppercase text-sm">
                    <th className="px-4 py-2 border">No</th>
                    <th className="px-4 py-2 border">Mã đơn hàng</th>
                    <th className="px-4 py-2 border">Lý do</th>
                    <th className="px-4 py-2 border">Trạng thái</th>
                    <th className="px-4 py-2 border">Chi nhánh</th>
                    <th className="px-4 py-2 border">Ngày tạo</th>
                    <th className="px-4 py-2 border">Ghi chú</th>
                  </tr>
                </thead>
                <tbody>
                  {refundRequests.$values.map((request, index) => (
                    <tr key={request.$id} className="hover:bg-gray-50">
                      <td className="px-4 py-2 border">{index+1}</td>
                      <td className="px-4 py-2 border">
                        {request.rentalOrderCode || "N/A"}
                      </td>
                      <td className="px-4 py-2 border">{request.reason}</td>
                      <td className="px-4 py-2 border">
                        <Chip
                          color={statusColors[request.status] || "gray"}
                          value={request.status}
                          icon={
                            request.status === "Pending" ? (
                              <FontAwesomeIcon icon={faHourglass} size="sm" />
                            ) : request.status === "Approved" ? (
                              <FontAwesomeIcon icon={faCheckCircle} size="sm" />
                            ) : (
                              <FontAwesomeIcon icon={faTimesCircle} size="sm" />
                            )
                          }
                        />
                      </td>
                      <td className="px-4 py-2 border">
                        {getBranchName(request.branchId)}
                      </td>
                      <td className="px-4 py-2 border">
                        {new Date(request.createdAt).toLocaleString("vi-VN")}
                      </td>
                      <td className="px-4 py-2 border">
                        {request.staffNotes || (
                          <span className="italic text-gray-400">Không có</span>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : (
            <p className="text-gray-500 text-center">
              Không có yêu cầu hoàn tiền nào.
            </p>
          )}
        </DialogBody>
        <DialogFooter>
          <Button color="red" onClick={toggleDialog} ripple={true}>
            Đóng
          </Button>
        </DialogFooter>
      </Dialog>
    </>
  );
};

export default RefundRequestPopup;
