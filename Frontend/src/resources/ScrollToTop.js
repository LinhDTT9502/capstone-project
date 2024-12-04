import { useEffect } from "react";
import { useLocation } from "react-router-dom";

const ScrollToTop = () => {
  const location = useLocation();

  useEffect(() => {
    // Mỗi khi location thay đổi, cuộn lên đầu trang
    window.scrollTo(0, 0);
  }, [location]);

  return null;
};

export default ScrollToTop;
