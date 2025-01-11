import React, { useState, useEffect } from "react";
import { fetchLikes, handleToggleLike } from "../../services/likeService";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faHeart as farHeart } from "@fortawesome/free-regular-svg-icons";
import { faHeart as fasHeart } from "@fortawesome/free-solid-svg-icons";
import { toast } from "react-toastify";
import { useSelector } from "react-redux";
import { selectUser } from "../../redux/slices/authSlice";

const LikeButton = ({
  productId,
  productCode,
  initialLikes,
  isLikedInitially,
}) => {
  const [likes, setLikes] = useState(initialLikes);
  const [isLiked, setIsLiked] = useState(isLikedInitially);
  const user = useSelector(selectUser);


  useEffect(() => {
    const fetchLikesData = async () => {
      try {
        const savedLikes = await fetchLikes(productCode);
        const likesCount = savedLikes.$values?.length || 0;
        if (user) {
          try {
            if (likesCount > 0) {
               const isUserIdExist = savedLikes.$values.some(
                 (item) => Number(item.userId) === Number(user.UserId) // Ép kiểu về number
               );
              if (isUserIdExist) {
                setIsLiked(true);
              }
            }
  
          } catch (error) {
            console.error("Error parsing JSON:", error);
          }
        }

        // const savedIsLiked = localStorage.getItem(`likeStatus-${productId}`);

        // if (savedIsLiked !== null) {
        //   setIsLiked(JSON.parse(savedIsLiked));
        // }

        if (likesCount !== undefined) {
          setLikes(likesCount);
        }
      } catch (error) {
        console.error("Lỗi khi fetch dữ liệu likes:", error);
      }
    };

    fetchLikesData(); // Gọi hàm fetch bên trong useEffect
  }, [productId, productCode]);

  const toggleLike = async () => {
    const storedToken = localStorage.getItem("token");
    if (!storedToken) {
      toast.info("Bạn cần đăng nhập để thích sản phẩm này");
      return;
    }
    const newIsLiked = !isLiked;
    const newLikes = newIsLiked ? likes + 1 : likes - 1;

    setIsLiked(newIsLiked);
    setLikes(newLikes);

    localStorage.setItem(`likeStatus-${productId}`, newIsLiked);
    localStorage.setItem(`likeCount-${productId}`, newLikes);

    try {
      await handleToggleLike(productCode);
    } catch (error) {
      console.error("Error toggling like:", error);
      setIsLiked(!newIsLiked);
      setLikes(newIsLiked ? likes - 1 : likes + 1);
    }
  };

  return (
    <div className="flex items-center space-x-2">
      <button
        onClick={toggleLike}
        className={`flex items-center justify-center px-4 py-2 rounded-full transition-colors duration-300 ${
          isLiked
            ? "bg-orange-500 text-white hover:bg-orange-600"
            : "bg-gray-200 text-gray-700 hover:bg-gray-300"
        }`}
      >
        <FontAwesomeIcon
          icon={isLiked ? fasHeart : farHeart}
          className="mr-2"
        />
        <span>{isLiked ? "Đã thích" : "Thích"}</span>
      </button>
      <span className="text-sm text-gray-600">{likes} lượt thích</span>
    </div>
  );
};

export default LikeButton;
