import { faFacebook, faInstagram } from "@fortawesome/free-brands-svg-icons";
import {
  faEnvelope,
  faLocationDot,
  faPhone,
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Typography } from "@material-tailwind/react";
import React from "react";
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";

function Footer() {
  const { t } = useTranslation();
  return (
    <div className="bg-zinc-700 h-full">
      {/* Banner Section */}
      <div className="bg-footer bg-cover bg-center flex justify-center items-center">
        <h1 className="text-3xl py-20 text-white font-alfa drop-shadow-md">
          {t("footer.text-img")}
          <Link
            to="https://fb.com/profile.php?id=61560697567321"
            className="underline underline-offset-2 pl-8"
          >Facebook
          </Link>
        </h1>
      </div>

      {/* Footer Content */}
      <div className="flex justify-between py-10 px-20 space-x-10">
        {/* Left Column */}
        <div className="w-1/3 text-white space-y-8">
          <img
            src="/assets/images/Logo.png"
            alt="2Sport"
            className="max-w-sm max-h-12"
          />
          <Typography>{t("footer.text")}</Typography>
          <Typography>{t("footer.Copyright")}</Typography>
        </div>

        {/* Middle Column - Contact Section */}
        <div className="w-1/3 text-white space-y-4">
          <Typography className="text-xl font-bold mb-4">
            {t("footer.contact")}
          </Typography>
          <div className="space-y-2">
            <Typography>
              <a
                href="https://www.facebook.com/profile.php?id=61560697567321&mibextid=LQQJ4d"
                className="flex items-center hover:text-blue-400 transition duration-300"
              >
                <FontAwesomeIcon icon={faFacebook} className="mr-2" size={20} />
                Facebook
              </a>
            </Typography>
          </div>

          <div className="space-y-2">
            <Typography>
              <a
                href="https://www.instagram.com/2sport_tt/"
                className="flex items-center hover:text-pink-400 transition duration-300"
              >
                <FontAwesomeIcon
                  icon={faInstagram}
                  className="mr-2"
                  size={20}
                />
                Instagram
              </a>
            </Typography>
          </div>
          <div className="flex items-center">
            <Typography>
              <FontAwesomeIcon
                icon={faLocationDot}
                className="mr-2"
                size={20}
              />
              {t("footer.address")}
            </Typography>
          </div>

          <div className="flex items-center">
            <Typography>
              <FontAwesomeIcon icon={faPhone} className="mr-2" size={20} />
              +84 338-581-571
            </Typography>
          </div>
          <div className="flex items-center">
            <Typography>
              <FontAwesomeIcon icon={faEnvelope} className="mr-2" size={20} />
              2sportteam@gmail.com
            </Typography>
          </div>
        </div>

        {/* Right Column - Policies Section */}
        <div className="w-1/3 text-white space-y-4">
          <h3 className="text-xl font-bold mb-4">Chính Sách</h3>
          <ul className="space-y-2">
            <li>
              <a
                href="/complaints-handling"
                className="hover:text-gray-300 transition duration-300"
              >
                Chính sách xử lý khiếu nại
              </a>
            </li>
            <li>
              <a
                href="/returns-refunds"
                className="hover:text-gray-300 transition duration-300"
              >
                Chính sách đổi trả, hoàn tiền
              </a>
            </li>
            <li>
              <a
                href="/payment"
                className="hover:text-gray-300 transition duration-300"
              >
                Chính sách thanh toán
              </a>
            </li>
            <li>
              <a
                href="/privacy"
                className="hover:text-gray-300 transition duration-300"
              >
                Chính sách bảo mật thông tin khách hàng
              </a>
            </li>
            <li>
              <a
                href="/membership"
                className="hover:text-gray-300 transition duration-300"
              >
                Chính sách dành cho membership khi thuê đồ tại 2Sport
              </a>
            </li>
            <li>
              <a
                href="/second-hand-rentals"
                className="hover:text-gray-300 transition duration-300"
              >
                Chính sách khi thuê đồ 2hand tại 2Sport
              </a>
            </li>
            <li>
              <a
                href="/shipping"
                className="hover:text-gray-300 transition duration-300"
              >
                Chính sách vận chuyển
              </a>
            </li>
          </ul>
        </div>
      </div>
    </div>
  );
}

export default Footer;
