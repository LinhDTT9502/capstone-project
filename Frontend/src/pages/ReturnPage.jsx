import { faCloudUploadAlt, faTimes } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Button } from "@material-tailwind/react";
import axios from "axios";
import { useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { toast } from "react-toastify";

const ReturnPage = () => {
  const location = useLocation();
  const { product, orderDetail } = location.state || {};
  const [selectedOption, setSelectedOption] = useState("");
  const [videoFile, setVideoFile] = useState(null);
  const [videoPreviewUrl, setVideoPreviewUrl] = useState(null);
  const [thumbnail, setThumbnail] = useState(null);
  const [description, setDescription] = useState(""); // Mô tả vấn đề
  const [loading, setLoading] = useState(false); 
  const navigate = useNavigate();

  const handleOptionChange = (event) => {
    setSelectedOption(event.target.value);
  };

  const handleVideoUpload = (e) => {
    const file = e.target.files[0];
    if (file) {
      setVideoFile(file);
      const previewUrl = URL.createObjectURL(file);
      setVideoPreviewUrl(previewUrl);
    }
  };

  const removeVideo = () => {
    setVideoFile(null);
    setVideoPreviewUrl(null);
    setThumbnail(null);
  };

  const handleSubmit = async () => {
    if (!selectedOption || !videoFile || !description) {
      toast.error("Vui lòng điền đầy đủ thông tin trước khi gửi yêu cầu!");
      return;
    }
    setLoading(true);
    const formData = new FormData();
    formData.append("Reason", selectedOption);
    formData.append("Notes", description || "");
    formData.append("Video", videoFile);
    formData.append("OrderId", orderDetail.id);
    formData.append(
      "ReturnAmount",
      orderDetail.tranSportFee + product.unitPrice
    );
    formData.append("ProductCode", product.productCode);
    formData.append("Size", product.size);
    formData.append("Color", product.color);
    formData.append("Condition", product.condition);

    try {
      const response = await axios.post(
        "https://twosport-api-offcial-685025377967.asia-southeast1.run.app//api/ReturnRequest", // Địa chỉ API
        formData,
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      );

      if (response) {
        toast.success("Yêu cầu trả hàng đã được gửi thành công!");
        navigate(-1);
      } else {
        toast.error("Đã xảy ra lỗi khi gửi yêu cầu trả hàng.");
      }
    } catch (error) {
      console.error("Lỗi khi gửi yêu cầu trả hàng:", error);
      toast.error("Đã xảy ra lỗi khi kết nối với máy chủ.");
    }finally{
      setLoading(false)
    }
  };

  return (
    <div className="p-6 bg-gray-100 min-h-screen">
      <div className="max-w-4xl mx-auto bg-white shadow-md rounded-lg p-6">
        {/* Tiêu đề */}
        <h2 className="text-lg font-semibold text-gray-800 border-b pb-4">
          Tình huống bạn đang gặp?
        </h2>

        {/* Thông tin đã chọn */}
        <div className="mt-4">
          <p className="text-sm text-gray-600">
            Tôi đã nhận hàng nhưng hàng có vấn đề (bể vỡ, sai mẫu, hàng lỗi,
            khác mô tả...) - Miễn ship hoàn về
          </p>
        </div>

        {/* Sản phẩm đã chọn */}
        <div className="mt-6 border-t pt-4">
          <h3 className="text-md font-semibold text-gray-800 mb-4">
            Sản phẩm đã chọn
          </h3>
          <div className="bg-gray-50 p-4 mb-4 rounded-lg shadow-sm">
            <div className="flex flex-col md:flex-row gap-4">
              <img
                src={product?.imgAvatarPath}
                alt={product?.productName}
                className="w-full md:w-32 h-32 object-cover rounded"
              />
              <div className="flex-grow">
                <h4 className="font-semibold text-lg mb-2 text-orange-500">
                  <Link to={`/product/${product?.productCode}`}>
                    {product?.productName}
                  </Link>
                </h4>
                <div className="grid grid-cols-2 gap-2">
                  <p>
                    <span className="font-semibold">Màu sắc:</span>{" "}
                    <i>{product?.color}</i>
                  </p>
                  <p>
                    <span className="font-semibold">Kích thước:</span>{" "}
                    <i>{product?.size}</i>
                  </p>
                  <p>
                    <span className="font-semibold">Đơn giá bán:</span>{" "}
                    <i>{product?.unitPrice?.toLocaleString("vi-VN")}₫</i>
                  </p>
                  <p>
                    <span className="font-semibold">Tình trạng:</span>{" "}
                    <i>{product?.condition}%</i>
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Lý do trả hàng */}
        <div className="mt-6 border-t pt-4">
          <h3 className="text-md font-semibold text-gray-800 mb-4">
            Chọn sản phẩm cần Trả hàng và Hoàn tiền
          </h3>
          <div className="space-y-4">
            {/* Chọn lý do */}
            <div>
              <label className="block text-sm font-medium text-gray-700">
                *Lý do
              </label>
              <select
                className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-red-500 focus:border-red-500 sm:text-sm"
                onChange={handleOptionChange}
              >
                <option value="">Chọn Lý Do</option>
                <option value="Giao sai hàng">Giao sai hàng</option>
                <option value="Hàng bị lỗi">Hàng bị lỗi</option>
                <option value="Hàng không đúng mô tả">
                  Hàng không đúng mô tả
                </option>
                <option value="Số lượng không đủ">Số lượng không đủ</option>
                <option value="Hàng giả">Hàng giả</option>
                <option value="Thiếu hàng">Thiếu hàng</option>
              </select>
            </div>

            {/* Tải video */}
            {selectedOption !== "" && (
              <div className="mt-2">
                <div className="flex justify-center px-6 pt-5 pb-5 border-2 border-gray-300 border-dashed rounded-md">
                  {videoFile ? (
                    <div className="relative">
                      <video
                        controls
                        src={videoPreviewUrl}
                        className="max-h-64 rounded-md"
                      />
                      <button
                        type="button"
                        onClick={removeVideo}
                        className="absolute top-1 right-1 text-white p-1 hover:text-red-500"
                      >
                        <FontAwesomeIcon icon={faTimes} className="h-5 w-5" />
                      </button>
                    </div>
                  ) : (
                    <div className="space-y-1 text-center">
                      <FontAwesomeIcon
                        icon={faCloudUploadAlt}
                        className="mx-auto h-12 w-12 text-gray-400"
                      />
                      <div className="flex text-sm text-gray-600">
                        <label
                          htmlFor="video-upload"
                          className="relative cursor-pointer bg-white rounded-md font-medium text-blue-600 hover:text-blue-500"
                        >
                          <span>Tải video lên</span>
                          <input
                            id="video-upload"
                            name="video-upload"
                            type="file"
                            accept="video/*"
                            className="sr-only"
                            onChange={handleVideoUpload}
                          />
                        </label>
                        <p className="pl-1">hoặc kéo và thả</p>
                      </div>
                      <p className="text-xs text-gray-500">MP4, AVI, MOV</p>
                    </div>
                  )}
                </div>
              </div>
            )}

            {/* Mô tả */}
            <div>
              <label className="block text-sm font-medium text-gray-700">
                Mô tả
              </label>
              <textarea
                className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-red-500 focus:border-red-500 sm:text-sm"
                rows="4"
                placeholder="Chi tiết vấn đề bạn gặp phải"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
              ></textarea>
              <p className="text-sm text-gray-500 text-right">
                {description.length}/2000
              </p>
            </div>
          </div>
        </div>
        <div className="mt-6 border-t pt-4">
          <h3 className="text-md font-semibold text-gray-800 mb-4">
            Thông tin hoàn tiền
          </h3>
          <div className="space-y-2">
            <p className="text-sm text-gray-700">
              Số tiền có thể hoàn lại:{" "}
              <span className="font-semibold">
                {product.unitPrice.toLocaleString("Vi-vn")}₫
              </span>
            </p>
            <p className="text-sm text-gray-700">
              Phí vận chuyển:{" "}
              <span className="font-semibold">
                {orderDetail.tranSportFee.toLocaleString("Vi-vn")}₫
              </span>
            </p>
            <p className="text-sm text-gray-700">
              Số tiền hoàn nhận được:{" "}
              <span className="font-semibold text-xl text-orange-500">
                {(orderDetail.tranSportFee + product.unitPrice).toLocaleString(
                  "Vi-vn"
                )}
                ₫
              </span>
            </p>
            <p className="text-sm text-gray-700">
              Hoàn tiền vào:{" "}
              <span className="font-semibold">
                2Sport sẽ liên hệ khách sau ạ
              </span>
            </p>
          </div>
        </div>
        <div className="mt-6 flex justify-end">
          <Button
            color="white"
            size="sm"
            className="bg-red-500 text-white py-2 px-6 rounded-md hover:bg-red-600"
            onClick={handleSubmit}
          >
            {loading ? "Đang xử lý..." : "Hoàn thành"}
          </Button>
        </div>
      </div>
    </div>
  );
};

export default ReturnPage;
