import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faGauge, faFileArrowDown } from "@fortawesome/free-solid-svg-icons";
import { useTranslation } from "react-i18next";
import { List, ListItem } from "@material-tailwind/react";
import { Link } from "react-router-dom";

export default function SidebarStaff() {
  const { t } = useTranslation();
  return (
    <div className="w-1/6  flex flex-col items-center  py-5 px-5">
      <List>
        <Link to="/admin/dashboard" >
        <ListItem>
        <FontAwesomeIcon icon={faGauge} className="pr-3" /> {t("sidebar_staff.dashboard")}
        </ListItem>
        </Link>
        {/* <ListItem>
          
        <FontAwesomeIcon icon={faFileArrowDown} className="pr-3" /> {t("sidebar_staff.import")}
        </ListItem> */}
      
      </List>
      
     
    </div>
  );
}
