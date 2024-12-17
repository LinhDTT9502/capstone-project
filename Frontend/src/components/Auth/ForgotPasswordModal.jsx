import { useEffect, useRef, useState } from 'react';
import { Dialog, Transition } from '@headlessui/react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEnvelope, faLock, faKey } from '@fortawesome/free-solid-svg-icons';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';
import { requestPasswordReset, performPasswordReset } from '../../services/authService';

export default function ResetPasswordModal({ isOpen, closeModal }) {
  const { register, handleSubmit, setValue, formState: { errors, isValid }, getValues } = useForm();
  const [loading, setLoading] = useState(false);
  const [step, setStep] = useState(1); 
  const [email, setEmail] = useState('');
  const [otpCode, setOtpCode] = useState(Array(6).fill('')); 
  const otpInputs = useRef([]);

  // Handle sending OTP request
  const handleEmailSubmit = async (data) => {
    setLoading(true);
    try {
      await requestPasswordReset(data.email); 
      toast.success('Đường link để reset mật khẩu đã được gửi tới email của bạn!');
      setEmail(data.email); 
      setStep(2); 
    } catch (error) {
      toast.error('Không thể gửi yêu cầu reset mật khẩu. Vui lòng thử lại.');
    } finally {
      setLoading(false);
    }
  };

  // Handle password reset
  const handlePasswordReset = async (data) => {
    setLoading(true);
    try {
      await performPasswordReset({ otpCode: otpCode.join(''), email, newPassword: data.newPassword });
      toast.success('Mật khẩu đã được đặt lại thành công!');
      closeModal();
    } catch (error) {
      toast.error('Không thể reset mật khẩu. Vui lòng thử lại.');
    } finally {
      setLoading(false);
    }
  };

  const handleOtpChange = (index, value) => {
    if (/[^0-9]/.test(value)) return; 
    const newOtpCode = [...otpCode];
    newOtpCode[index] = value;
    setOtpCode(newOtpCode);

    if (value && index < 5) {
      otpInputs.current[index + 1]?.focus();
    }
  };

  // Resend OTP
  const handleResendOtp = async () => {
    setLoading(true);
    try {
      await requestPasswordReset(email); 
      toast.success('OTP đã được gửi lại tới email của bạn!');
    } catch (error) {
      toast.error('Không thể gửi lại OTP. Vui lòng thử lại.');
    } finally {
      setLoading(false);
    }
  };

  // Change email
  const handleChangeEmail = () => {
    setStep(1); 
    setOtpCode(Array(6).fill('')); 
  };

  return (
    <Transition appear show={isOpen} as="div">
      <Dialog as="div" className="relative z-[999]" onClose={closeModal}>
        <Transition.Child
          as="div"
          enter="ease-out duration-300"
          enterFrom="opacity-0"
          enterTo="opacity-100"
          leave="ease-in duration-200"
          leaveFrom="opacity-100"
          leaveTo="opacity-0"
        >
          <div className="fixed inset-0 bg-black bg-opacity-25" />
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
<Dialog.Panel className="w-full max-w-2xl transform overflow-hidden rounded-2xl bg-white p-8 text-left align-middle shadow-xl transition-all">
<Dialog.Title as="h3" className="text-2xl font-medium leading-6 text-gray-900 mb-6 text-center">
                  Đặt lại mật khẩu
                </Dialog.Title>

                {/* Step 1: Enter email */}
                {step === 1 && (
                  <form onSubmit={handleSubmit(handleEmailSubmit)} className="space-y-4">
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">Email của bạn</label>
                      <div className="relative">
                        <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                          <FontAwesomeIcon icon={faEnvelope} className="text-gray-400" />
                        </div>
                        <input
                          type="email"
                          placeholder="Nhập email của bạn"
                          className="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-orange-500 sm:text-sm"
                          {...register('email', {
                            required: 'Email là bắt buộc',
                            pattern: {
                              value: /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/,
                              message: 'Định dạng email không hợp lệ',
                            },
                          })}
                        />
                      </div>
                      {errors.email && <p className="mt-1 text-sm text-red-600">{errors.email.message}</p>}
                    </div>

                    <button
                      type="submit"
                      className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-orange-600 hover:bg-orange-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-orange-500 transition duration-150 ease-in-out"
                      disabled={loading}
                    >
                      {loading ? 'Đang tải...' : 'Gửi yêu cầu reset mật khẩu'}
                    </button>
                  </form>
                )}

                {/* Step 2: Enter OTP and new password */}
                {step === 2 && (
                  <form onSubmit={handleSubmit(handlePasswordReset)} className="space-y-4">
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">Mã OTP</label>
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
                      {errors.otpCode && <p className="mt-1 text-sm text-red-600">{errors.otpCode.message}</p>}
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">Mật khẩu mới</label>
                      <div className="relative">
                        <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                          <FontAwesomeIcon icon={faLock} className="text-gray-400" />
                        </div>
                        <input
                          type="password"
                          placeholder="Nhập mật khẩu mới"
                          className="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-orange-500 sm:text-sm"
                          {...register('newPassword', {
                            required: 'Mật khẩu là bắt buộc',
                            minLength: {
                              value: 8,
                              message: 'Mật khẩu phải có ít nhất 8 ký tự',
                            },
                          })}
                        />
                      </div>
                      {errors.newPassword && <p className="mt-1 text-sm text-red-600">{errors.newPassword.message}</p>}
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">Xác nhận mật khẩu</label>
                      <div className="relative">
                        <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                          <FontAwesomeIcon icon={faKey} className="text-gray-400" />
                        </div>
                        <input
                          type="password"
                          placeholder="Xác nhận mật khẩu của bạn"
                          className="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-orange-500 sm:text-sm"
                          {...register('confirmPassword', {
                            required: 'Vui lòng xác nhận mật khẩu',
                            validate: (value) => value === getValues('newPassword') || "Mật khẩu không khớp",
                          })}
                        />
                      </div>
                      {errors.confirmPassword && <p className="mt-1 text-sm text-red-600">{errors.confirmPassword.message}</p>}
                    </div>

                    <button
                      type="submit"
                      className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-orange-600 hover:bg-orange-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-orange-500 transition duration-150 ease-in-out"
                      disabled={loading || !isValid}
                    >
                      {loading ? 'Đang tải...' : 'Đặt lại mật khẩu'}
                    </button>

                    <div className="flex justify-between mt-4">
                      <button
                        type="button"
                        onClick={handleResendOtp}
                        className="text-sm text-orange-600 hover:text-orange-700"
                        disabled={loading}
                      >
                        Gửi lại OTP
                      </button>
                      <button
                        type="button"
                        onClick={handleChangeEmail}
                        className="text-sm text-gray-600 hover:text-gray-800"
                      >
                        Thay đổi email
                      </button>
                    </div>
                  </form>
                )}
              </Dialog.Panel>
            </Transition.Child>
          </div>
        </div>
      </Dialog>
    </Transition>
  );
}
