import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faStar as fasStar } from '@fortawesome/free-solid-svg-icons';
import { faStar as farStar } from '@fortawesome/free-regular-svg-icons';

const StarRating = ({ rating, onRatingChange }) => {
  return (
    <div className="flex items-center space-x-1">
      {[1, 2, 3, 4, 5].map((star) => (
        <FontAwesomeIcon
          key={star}
          icon={star <= rating ? fasStar : farStar}
          className={`cursor-pointer text-2xl ${
            star <= rating ? 'text-yellow-400' : 'text-gray-300'
          }`}
          onClick={() => onRatingChange(star)}
        />
      ))}
    </div>
  );
};

export default StarRating;

