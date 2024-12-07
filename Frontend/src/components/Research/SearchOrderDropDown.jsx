import { useSelector } from "react-redux";
import { selectUser } from "../../redux/slices/authSlice";
import { motion } from "framer-motion";
import { useState, useRef } from "react";
import { useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBinoculars, faCaretDown, faCaretUp } from "@fortawesome/free-solid-svg-icons";
import { useTranslation } from "react-i18next";

const itemVariants = {
  open: {
    opacity: 1,
    y: 0,
    transition: { type: "spring", stiffness: 300, damping: 24 },
  },
  closed: { opacity: 0, y: 20, transition: { duration: 0.2 } },
};

export default function SearchOrderDropDown() {
  const { t } = useTranslation();
  const user = useSelector(selectUser);
  const [isOpen, setIsOpen] = useState(false);
  const navigate = useNavigate();
  const timeoutRef = useRef(null);

  const handleOrderList = () => {
    if (user) {
      navigate("/manage-account/sale-order");
    } else {
      navigate("/guest-order");
    }
  };

  const handleRentalList = () => {
    if (user) {
      navigate("/manage-account/user-rental");
    } else {
      navigate("/guest-order");
    }
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
      className="relative inline-block"
      onMouseLeave={handleMouseLeave}
    >
      <div
        onMouseEnter={handleMouseEnter}
        className="flex items-center cursor-pointer justify-center border-r-2 pr-4"
      >
        <FontAwesomeIcon icon={faBinoculars} className="pr-2" />
        <span>Tra cứu</span>
        <motion.div transition={{ duration: 0.2 }} className="pl-2">
          {isOpen ? (
            <FontAwesomeIcon icon={faCaretUp} />
          ) : (
            <FontAwesomeIcon icon={faCaretDown} />
          )}
        </motion.div>
      </div>
      <motion.ul
        className="absolute left-1/2 transform -translate-x-1/2 mt-2 backdrop-blur-lg text-white bg-gradient-to-r from-zinc-600/80 to-zinc-900/80 rounded-md shadow-lg w-40 "
        variants={{
          open: {
            clipPath: "inset(0% 0% 0% 0% round 10px)",
            transition: {
              type: "spring",
              bounce: 0,
              duration: 0.7,
              delayChildren: 0.3,
              staggerChildren: 0.05,
            },
          },
          closed: {
            clipPath: "inset(10% 50% 90% 50% round 10px)",
            transition: {
              type: "spring",
              bounce: 0,
              duration: 0.3,
            },
          },
        }}
        style={{ pointerEvents: isOpen ? "auto" : "none" }}
        onMouseEnter={handleMouseEnter}
        onMouseLeave={handleMouseLeave}
      >
        <motion.li
          variants={itemVariants}
          className="p-3 border-b-2 border-white cursor-pointer"
          onClick={handleOrderList}
        >
          <button>Kiểm tra đơn mua</button>
        </motion.li>
        <motion.li
          variants={itemVariants}
          className="p-3 cursor-pointer"
          onClick={handleRentalList}
        >
          <button>Kiểm tra đơn thuê</button>
        </motion.li>
      </motion.ul>
    </motion.nav>
  );
}
