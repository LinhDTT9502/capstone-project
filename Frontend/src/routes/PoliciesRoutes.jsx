import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import ComplaintsHandlingPage from '../pages/ComplaintsHandlingPage ';
import ReturnsRefundsPage from '../pages/ReturnsRefundsPage';
import PaymentPolicyPage from '../pages/PaymentPolicyPage';
import MembershipPolicyPage from '../pages/MembershipPolicyPage';
import SecondHandRentalsPolicyPage from '../pages/SecondHandRentalsPolicyPage';
import ShippingPolicyPage from '../pages/ShippingPolicyPage';
import PrivacyPolicyPage from "../pages/Policies/PrivacyPolicyPage";
// import other policy pages

function PoliciesRoutes() {
  return (
    <Router>
      <Routes>
        <Route path="/complaints-handling" element={<ComplaintsHandlingPage />} />
        <Route path="/returns-refunds" element={<ReturnsRefundsPage />} />
        <Route path="/payment" element={<PaymentPolicyPage />} />
        <Route path="/privacy" element={<PrivacyPolicyPage />} />
        <Route path="/membership" element={<MembershipPolicyPage />} />
        <Route path="/second-hand-rentals" element={<SecondHandRentalsPolicyPage />} />
        <Route path="/shipping" element={<ShippingPolicyPage />} />
      </Routes>
    </Router>
  );
}

export default PoliciesRoutes;
