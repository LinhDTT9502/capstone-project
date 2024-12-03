import { faFacebook, faInstagram } from "@fortawesome/free-brands-svg-icons";
import { faEnvelope, faLocationDot, faPhone } from "@fortawesome/free-solid-svg-icons";
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
          >
            Facebook
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
          <Typography className="font-alfa">{t("footer.contact")}</Typography>
          <Typography>
            <a
              href="https://www.facebook.com/profile.php?id=61560697567321&mibextid=LQQJ4d"
              className="text-white"
            >
              <FontAwesomeIcon icon={faFacebook} className="pr-1" />
              Facebook
            </a>
          </Typography>
          <Typography>
            <a
              href="https://www.instagram.com/2sport_tt/"
              className="text-white"
            >
              <FontAwesomeIcon icon={faInstagram} className="pr-1" />
              Instagram
            </a>
          </Typography>
          <Typography>
            <FontAwesomeIcon icon={faLocationDot} className="pr-1" />
            {t("footer.address")}
          </Typography>
          <Typography>
            <FontAwesomeIcon icon={faPhone} className="pr-1" />
            +84 338-581-571
          </Typography>
          <Typography>
            <FontAwesomeIcon icon={faEnvelope} className="pr-1" />
            2sportteam@gmail.com
          </Typography>
        </div>

        {/* Right Column - Policies Section */}
        <div className="w-1/3 text-white space-y-4">
          <Typography className="font-alfa text-lg">Chính Sách</Typography>
          <ul className="list-disc list-inside space-y-2">
            <li>Chính sách xử lý khiếu nại</li>
            <li>Chính sách đổi trả, hoàn tiền</li>
            <li>Chính sách thanh toán</li>
            <li>Chính sách bảo mật thông tin khách hàng</li>
            <li>Chính sách dành cho membership khi thuê đồ tại 2Sport</li>
            <li>Chính sách khi thuê đồ 2hand tại 2Sport</li>
            <li>Chính sách vận chuyển</li>
          </ul>
        </div>
      </div>
    </div>
  );
}

export default Footer;
