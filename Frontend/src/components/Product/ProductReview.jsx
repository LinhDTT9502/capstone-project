import React, { useState, useEffect } from 'react';
import { fetchReviewsByProductCode } from '../../services/reviewService';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faStar, faSpinner } from '@fortawesome/free-solid-svg-icons';
import { selectUser } from "../../redux/slices/authSlice";
import { useSelector } from 'react-redux';

const ProductReviews = ({ productCode }) => {
  const [reviews, setReviews] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
const user = useSelector(selectUser);


  useEffect(() => {
    const loadReviews = async () => {
      try {
        setLoading(true);
        const fetchedReviews = await fetchReviewsByProductCode(productCode);
        setReviews(fetchedReviews);
      } catch (err) {
        setError('Failed to load reviews. Please try again later.');
      } finally {
        setLoading(false);
      }
    };

    loadReviews();
  }, [productCode]);

  if (loading) {
    return (
      <div className="flex justify-center items-center h-24">
        <FontAwesomeIcon icon={faSpinner} spin size="2x" className="text-orange-500" />
      </div>
    );
  }

  if (error) {
    return <div className="text-red-500 text-center">{error}</div>;
  }

  if (reviews.length === 0) {
    return <div className="text-gray-500 text-center">Sản phẩm chưa có review nào.</div>;
  }

  return (
    <div className="mt-8 mb-2">
      <h2 className="text-2xl font-bold mb-4">Đánh giá của khách hàng</h2>
      <div className="space-y-4">
        {reviews.map((review, index) => (
            
          <div key={index} className="bg-white p-4 rounded-lg shadow">
            <div className="flex items-center mb-2">
              <div className="font-bold mr-2">{review.userName}</div>
              
              <div className="text-yellow-400">
                {[...Array(5)].map((_, i) => (
                  <FontAwesomeIcon
                    key={i}
                    icon={faStar}
                    className={i < review.star ? 'text-yellow-400' : 'text-gray-300'}
                  />
                ))}
              </div>
            </div>
            <p className="text-gray-600">{review.reviewContent}</p>
            {/* <div className="text-sm text-gray-400 mt-2">
              {new Date(review.createdAt).toLocaleDateString()}
            </div> */}
          </div>
        ))}
      </div>
    </div>
  );
};

export default ProductReviews;
