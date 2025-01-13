import { Dialog, Transition } from "@headlessui/react";
import { Fragment, useState, useRef, useEffect } from "react";
import { useDispatch } from "react-redux";
import { login } from "../../redux/slices/authSlice";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEye, faEyeSlash, faTimes, faXmark } from "@fortawesome/free-solid-svg-icons";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";
import LoginGoogle from "./LoginGoogle";
import { signUpUser, verifyAccountMobile } from "../../services/authService";
import { useTranslation } from "react-i18next";
import ReCAPTCHA from "react-google-recaptcha";
import { Link } from "react-router-dom";

export default function SignUpModal({ isOpen, closeModal, openSignInModal }) {
  const [captcha, setCaptcha] = useState(null);
  const siteKey = import.meta.env.VITE_SITE_KEY_CAPTCHA;
  const { t } = useTranslation();
  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
    reset,
  } = useForm();
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [otpModalVisible, setOtpModalVisible] = useState(false);
  const [otpCode, setOtpCode] = useState(["", "", "", "", "", ""]);
  const dispatch = useDispatch();
  const otpInputs = useRef([]);

  const togglePasswordVisibility = () => {
    setShowPassword(!showPassword);
  };

  const toggleConfirmPasswordVisibility = () => {
    setShowConfirmPassword(!showConfirmPassword);
  };

  const onSubmit = async (data) => {
    const { username, password, fullName, email, acceptTerms } = data;

    if (!acceptTerms) {
      toast.error(t("SignUpModal.accept_terms_error"));
      return;
    }

    try {
      const response = await signUpUser({
        username,
        password,
        fullName,
        email,
      });
      console.log("Sign-up successful:", response);
      toast.success("Đăng ký thành công");
      setOtpModalVisible(true);
    } catch (error) {
      console.error("Sign-up failed:", error);
      toast.error("Đăng ký thất bại, vui lòng thử lại");
    }
  };

  const handleVerifyOtp = async () => {
    const otpString = otpCode.join("");
    if (otpString.length !== 6) {
      toast.error("Vui lòng nhập đủ 6 số OTP!");
      return;
    }

    try {
      await verifyAccountMobile({
        username: watch("username"),
        email: watch("email"),
        OtpCode: otpString,
      });
      toast.success("Tài khoản đã được xác thực.");
      setOtpModalVisible(false);
      closeModal();
    } catch (error) {
      toast.error("Mã OTP không chính xác hoặc đã hết hạn.");
    }
  };

  const handleOtpChange = (index, value) => {
    if (!/^\d$/.test(value) && value !== "") return;
    const newOtpCode = [...otpCode];
    newOtpCode[index] = value;
    setOtpCode(newOtpCode);

    if (value && index < 5) {
      otpInputs.current[index + 1]?.focus();
    }
  };

  const handleOtpInputFocus = (e) => {
    e.stopPropagation();
  };

  const handleSignInClick = () => {
    openSignInModal();
    closeModal();
  };

  const handleCloseOtpModal = () => {
    setOtpModalVisible(false);
  };

  const resendVerificationEmail = async () => {
    try {
      await fetch('https://twosport-api-offcial-685025377967.asia-southeast1.run.app//api/User/send-verification-email', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          email: watch("email")
        })
      });
      toast.success("Mã OTP đã được gửi lại!");
    } catch (error) {
      toast.error("Không thể gửi lại mã OTP. Vui lòng thử lại sau.");
    }
  };

  useEffect(() => {
    if (isOpen) {
      reset();
      setOtpCode(["", "", "", "", "", ""]);
      setOtpModalVisible(false);
      setShowPassword(false);
      setShowConfirmPassword(false);
      setCaptcha(null);
    }
  }, [isOpen, reset]);

  return (
    <Transition appear show={isOpen} as="div">
      <Dialog
        as="div"
        className="relative z-10"
        onClose={(e) => e.stopPropagation()}
      >
        <Transition.Child
          as="div"
          enter="ease-out duration-300"
          enterFrom="opacity-0"
          enterTo="opacity-100"
          leave="ease-in duration-200"
          leaveFrom="opacity-100"
          leaveTo="opacity-0"
        >
          <div className="fixed inset-0 bg-black/25" />
        </Transition.Child>

        <div className="fixed inset-0 overflow-y-auto">
          <div className="flex min-h-full items-center justify-center pt-20">
            <Transition.Child
              as="div"
              enter="ease-out duration-300"
              enterFrom="opacity-0 scale-95"
              enterTo="opacity-100 scale-100"
              leave="ease-in duration-200"
              leaveFrom="opacity-100 scale-100"
              leaveTo="opacity-0 scale-95"
            >
              <Dialog.Panel className="flex justify-between w-full max-w-5xl transform overflow-hidden rounded-md shadow-xl transition-all">
                <div className="flex-col flex px-14 space-y-5 bg-zinc-700 text-white items-center justify-center w-1/2">
                  <h1 className="font-alfa text-lg text-orange-500">
                    {t("SignUpModal.Come join us!")}
                  </h1>
                  <p className="font-poppins text-center">
                    {t("SignUpModal.welcome_message")}
                  </p>
                  <button
                    className="flex font-poppins bg-gradient-to-r from-zinc-500 to-zinc-600 w-fit p-3 shadow-zinc-800 shadow-md rounded-md"
                    onClick={handleSignInClick}
                  >
                    {t("SignUpModal.have_account")}{" "}
                    <p className="pl-1 text-orange-500 font-bold">
                      {t("SignUpModal.Signin!")}
                    </p>
                  </button>
                </div>

                <div className="bg-white w-1/2 px-20 text-black flex-col flex font-poppins justify-center py-10">
                  <button
                    onClick={closeModal}
                    className="absolute top-5 right-5 text-gray-500 hover:text-gray-700"
                  >
                    <FontAwesomeIcon icon={faXmark} className="w-5 h-5" />
                  </button>
                  <form
                    onSubmit={handleSubmit(onSubmit)}
                    className="space-y-2 text-black flex-col flex font-poppins justify-center"
                  >
                    <label className="space-y-3 font-alfa text-xl items-center text-center mb-2">
                      {t("SignUpModal.Signup")}
                    </label>

                    <input
                      type="text"
                      placeholder={t("SignUpModal.placeholder_fullname")}
                      className="text-gray-700 p-2 rounded-lg border-2 border-zinc-400 w-full"
                      {...register("fullName", { required: true })}
                    />
                    {errors.fullName && (
                      <p className="text-red-400 text-sm italic ">
                        {t("SignUpModal.fullname")}
                      </p>
                    )}

                    <input
                      type="text"
                      placeholder={t("SignUpModal.placeholder_username")}
                      className="text-gray-700 p-2 rounded-lg border-2 border-zinc-400 w-full"
                      {...register("username", {
                        required: true,
                        maxLength: 20,
                        pattern: /^[a-zA-Z0-9_]+$/,
                      })}
                    />
                    {errors.username && errors.username.type === "required" && (
                      <p className="text-red-400 text-sm italic">
                        {t("SignUpModal.required")}
                      </p>
                    )}
                    {errors.username &&
                      errors.username.type === "maxLength" && (
                        <p className="text-red-400 text-sm italic">
                          {t("SignUpModal.maxLength")}
                        </p>
                      )}
                    {errors.username && errors.username.type === "pattern" && (
                      <p className="text-red-400 text-sm italic">
                        {t("SignUpModal.validate_username")}
                      </p>
                    )}

                    <input
                      type="email"
                      placeholder={t("SignUpModal.placeholder_email")}
                      className="text-gray-700 p-2 rounded-lg border-2 border-zinc-400 w-full"
                      {...register("email", {
                        required: "Email is required",
                        pattern: {
                          value: /^\S+@\S+$/i,
                          message: "Invalid email address",
                        },
                      })}
                    />
                    {errors.email && (
                      <p className="text-red-400 text-sm italic">
                        {errors.email.message}
                      </p>
                    )}

                    <div className="relative">
                      <input
                        type={showPassword ? "text" : "password"}
                        placeholder={t("SignUpModal.placeholder_password")}
                        className="text-gray-700 p-2 rounded-lg border-2 border-zinc-400 w-full"
                        {...register("password", { required: true })}
                      />
                      <div className="absolute inset-y-0 right-0 pr-3 flex items-center">
                        <FontAwesomeIcon
                          icon={showPassword ? faEyeSlash : faEye}
                          className="cursor-pointer text-orange-400"
                          onClick={togglePasswordVisibility}
                        />
                      </div>
                    </div>
                    {errors.password && (
                      <p className="text-red-400 text-sm italic">
                        {t("SignUpModal.validate_password")}
                      </p>
                    )}

                    <div className="relative">
                      <input
                        type={showConfirmPassword ? "text" : "password"}
                        placeholder={t(
                          "SignUpModal.placeholder_confirmPassword"
                        )}
                        className="text-gray-700 p-2 rounded-lg border-2 border-zinc-400 w-full"
                        {...register("confirmPassword", {
                          required: true,
                          validate: (value) => value === watch("password"),
                        })}
                      />
                      <div className="absolute inset-y-0 right-0 pr-3 flex items-center">
                        <FontAwesomeIcon
                          icon={showConfirmPassword ? faEyeSlash : faEye}
                          className="cursor-pointer text-orange-400"
                          onClick={toggleConfirmPasswordVisibility}
                        />
                      </div>
                    </div>
                    {errors.confirmPassword && (
                      <p className="text-red-400 text-sm italic">
                        {errors.confirmPassword.type === "required"
                          ? t("SignUpModal.validate_password")
                          : t("SignUpModal.passwords_not_match")}
                      </p>
                    )}

                    <div className="flex items-center">
                      <input
                        type="checkbox"
                        id="acceptTerms"
                        {...register("acceptTerms", { required: true })}
                        className="mr-2"
                      />
                      <label htmlFor="acceptTerms" className="text-sm flex">
                        Đồng ý với các{' '}
                        <Link to="/complaints-handling" onClick={closeModal} className="font-bold text-blue-500 ml-1">
                          chính sách của 2Sport
                        </Link>
                      </label>

                    </div>
                    {errors.acceptTerms && (
                      <p className="text-red-400 text-sm italic">
                        Vui lòng đồng ý với các chính sách của 2Sport để tiếp
                        tục
                      </p>
                    )}

                    <button
                      type="submit"
                      className="text-white w-full bg-gradient-to-r from-orange-500 to-orange-600 p-3 rounded-md mt-4"
                    >
                      Đăng ký
                    </button>
                  </form>
                </div>
              </Dialog.Panel>
            </Transition.Child>
          </div>
        </div>
      </Dialog>

      <Transition appear show={otpModalVisible} as="div">
        <Dialog
          as="div"
          className="relative z-10"
          onClose={(e) => e.stopPropagation()}
        >
          <Transition.Child
            as="div"
            enter="ease-out duration-300"
            enterFrom="opacity-0"
            enterTo="opacity-100"
            leave="ease-in duration-200"
            leaveFrom="opacity-100"
            leaveTo="opacity-0"
          >
            <div className="fixed inset-0 bg-black/25" />
          </Transition.Child>

          <div className="fixed inset-0 overflow-y-auto">
            <div className="flex min-h-full items-center justify-center p-4 text-center">
              <Transition.Child
                as="div"
                enter="ease-out duration-300"
                enterFrom="opacity-0 scale-95"
                enterTo="opacity-100 scale-100"
                leave="ease-in duration-200"
                leaveFrom="opacity-100 scale-100"
                leaveTo="opacity-0 scale-95"
              >
                <Dialog.Panel className="bg-white p-6 rounded-lg shadow-xl w-full max-w-md relative"
                  onClick={(e) => e.stopPropagation()}>
                  <button
                    onClick={closeModal}
                    className="absolute top-2 right-2 text-gray-500 hover:text-gray-700"
                  >
                    <FontAwesomeIcon icon={faXmark} className="w-5 h-5" />
                  </button>
                  <Dialog.Title
                    as="h3"
                    className="text-lg font-medium leading-6 text-gray-900 mb-4"
                  >
                    Xác thực OTP
                  </Dialog.Title>
                  <p className="text-sm text-gray-500 mb-6">
                    Nhập mã OTP
                  </p>
                  <div className="flex justify-center space-x-2 mb-6">
                    {otpCode.map((digit, index) => (
                      <input
                        key={index}
                        type="text"
                        maxLength={1}
                        value={digit}
                        onChange={(e) => handleOtpChange(index, e.target.value)}
                        ref={(el) => (otpInputs.current[index] = el)}
                        className="w-12 h-12 text-center text-xl border-2 border-gray-300 rounded-md focus:border-orange-500 focus:outline-none"
                      />
                    ))}
                  </div>
                  <div className="mt-4 flex justify-between items-center">
                    <button
                      type="button"
                      className="text-sm text-gray-500 hover:text-gray-700"
                      onClick={resendVerificationEmail}
                    >
                      Gửi lại mã OTP
                    </button>
                    <div className="flex gap-2">
                      <button
                        type="button"
                        className="inline-flex justify-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50"
                        onClick={handleCloseOtpModal}
                      >
                        Cancel
                      </button>
                      <button
                        type="button"
                        className="inline-flex justify-center rounded-md border border-transparent bg-orange-100 px-4 py-2 text-sm font-medium text-orange-900 hover:bg-orange-200 focus:outline-none focus-visible:ring-2 focus-visible:ring-orange-500 focus-visible:ring-offset-2"
                        onClick={handleVerifyOtp}
                      >
                        Xác thực
                      </button>
                    </div>
                  </div>
                </Dialog.Panel>
              </Transition.Child>
            </div>
          </div>
        </Dialog>
      </Transition>
    </Transition>
  );
}

