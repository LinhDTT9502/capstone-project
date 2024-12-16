import { useSelector } from "react-redux";
import { selectUser } from "../../redux/slices/authSlice";
import { motion } from "framer-motion";
import { useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";
import { logout } from "../../redux/slices/authSlice";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faRightFromBracket, faCaretDown, faCaretUp, faUser } from '@fortawesome/free-solid-svg-icons';
import Logout from "../Auth/Logout";
import { useTranslation } from "react-i18next";

const itemVariants = {
    open: {
        opacity: 1,
        y: 0,
        transition: { type: "spring", stiffness: 300, damping: 24 }
    },
    closed: { opacity: 0, y: 20, transition: { duration: 0.2 } }
};

export default function UserDropdown() {
    const { t } = useTranslation();
    const user = useSelector(selectUser);
    const [isOpen, setIsOpen] = useState(false);
    const navigate = useNavigate();
    const dispatch = useDispatch();
    const timeoutRef = useRef(null);

    const handleManageAccount = () => {
        navigate("/manage-account/profile");
    };

    const handleMouseEnter = () => {
        if (timeoutRef.current) {
          clearTimeout(timeoutRef.current);
          timeoutRef.current = null;
        }
        setIsOpen(true);
      };
    
      const handleMouseLeave = () => {
        timeoutRef.current = setTimeout(() => {
          setIsOpen(false);
        }, 500); 
      };
      

    return (
        <motion.nav
            initial={false}
            animate={isOpen ? "open" : "closed"}
            className="menu"
            onMouseLeave={handleMouseLeave}
        >
            <div 
              onMouseEnter={handleMouseEnter}
            className="justify-between flex text-left items-center border-r-2 pr-4">
                <button>
                <FontAwesomeIcon icon={faUser} className="pr-1" />
                {user.FullName}
                </button>
                
                <motion.div
                    transition={{ duration: 0.2 }}
                >
                    {isOpen ? (
                        <FontAwesomeIcon icon={faCaretUp} className="pl-2" />
                    ) : (
                        <FontAwesomeIcon icon={faCaretDown} className="pl-2" />
                    )}
                </motion.div>
            </div>
          
            <motion.ul
                className="absolute mt-2 backdrop-blur-lg text-white bg-gradient-to-r from-zinc-600/80 rounded-md shadow-lg to-zinc-900/80"
                variants={{
                    open: {
                        clipPath: "inset(0% 0% 0% 0% round 10px)",
                        transition: {
                            type: "spring",
                            bounce: 0,
                            duration: 0.7,
                            delayChildren: 0.3,
                            staggerChildren: 0.05
                        }
                    },
                    closed: {
                        clipPath: "inset(10% 50% 90% 50% round 10px)",
                        transition: {
                            type: "spring",
                            bounce: 0,
                            duration: 0.3
                        }
                    }
                }}
                style={{ pointerEvents: isOpen ? "auto" : "none" }}
                onMouseEnter={handleMouseEnter}
                onMouseLeave={handleMouseLeave}
            >
                <motion.li
                    variants={itemVariants}
                    className="p-3  border-b-2 border-white"
                    onClick={handleManageAccount}
                >
                    <button>
                        <FontAwesomeIcon icon={faUser} className="pr-1" />
                        {t("user_dropdown.manage_my_account")}
                    </button>
                </motion.li>    
                <motion.li
                    variants={itemVariants}
                    className=" p-3 "
                   
                >
                    <Logout />
                </motion.li>
            </motion.ul>
        </motion.nav>
    );
}
