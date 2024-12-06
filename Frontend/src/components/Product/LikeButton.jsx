import React, { useState, useEffect } from "react";
import { handleToggleLike } from "../../services/likeService";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faHeart as farHeart } from "@fortawesome/free-regular-svg-icons";
import { faHeart as fasHeart } from "@fortawesome/free-solid-svg-icons";

const LikeButton = ({ productId, initialLikes, isLikedInitially }) => {
  const [likes, setLikes] = useState(initialLikes);
  const [isLiked, setIsLiked] = useState(isLikedInitially);

  useEffect(() => {
    const savedIsLiked = localStorage.getItem(`likeStatus-${productId}`);
    const savedLikes = localStorage.getItem(`likeCount-${productId}`);

    if (savedIsLiked !== null) {
      setIsLiked(JSON.parse(savedIsLiked));
      setLikes(Number(savedLikes));
    }
  }, [productId]);

  const toggleLike = async () => {
    const newIsLiked = !isLiked;
    const newLikes = newIsLiked ? likes + 1 : likes - 1;

    setIsLiked(newIsLiked);
    setLikes(newLikes);

    localStorage.setItem(`likeStatus-${productId}`, newIsLiked);
    localStorage.setItem(`likeCount-${productId}`, newLikes);

    try {
      await handleToggleLike(productId);
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
        <FontAwesomeIcon icon={isLiked ? fasHeart : farHeart} className="mr-2" />
        <span>{isLiked ? "Đã thích" : "Thích"}</span>
      </button>
      <span className="text-sm text-gray-600">{likes} lượt thích</span>
    </div>
  );
};

export default LikeButton;
