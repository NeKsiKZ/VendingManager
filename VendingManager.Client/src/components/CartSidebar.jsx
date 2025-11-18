import { useCart } from '../context/CartContext';

function CartSidebar({ onCheckout }) {
    const { cartItems, removeFromCart, cartTotal, isCartOpen, setIsCartOpen } = useCart();

    if (!isCartOpen) return null;

    return (
        <>
            {/* Tło przyciemniające */}
            <div 
                className="position-fixed top-0 start-0 w-100 h-100 bg-dark bg-opacity-50" 
                style={{ zIndex: 1040 }}
                onClick={() => setIsCartOpen(false)}
            ></div>

            {/* Panel boczny */}
            <div 
                className="position-fixed top-0 end-0 h-100 bg-white shadow p-4 d-flex flex-column" 
                style={{ zIndex: 1050, width: '350px', maxWidth: '100%' }}
            >
                <div className="d-flex justify-content-between align-items-center mb-4">
                    <h4 className="mb-0">Twój Koszyk</h4>
                    <button className="btn-close" onClick={() => setIsCartOpen(false)}></button>
                </div>

                <div className="flex-grow-1 overflow-auto">
                    {cartItems.length === 0 ? (
                        <p className="text-muted text-center mt-5">Koszyk jest pusty.</p>
                    ) : (
                        <ul className="list-group list-group-flush">
                            {cartItems.map((item, index) => (
                                <li key={index} className="list-group-item d-flex justify-content-between align-items-center px-0">
                                    <div>
                                        <div className="fw-bold">{item.productName}</div>
                                        <small className="text-muted">
                                            {item.count} x {item.price.toFixed(2)} PLN
                                        </small>
                                    </div>
                                    <div className="d-flex align-items-center">
                                        <span className="fw-bold me-3">
                                            {(item.count * item.price).toFixed(2)} zł
                                        </span>
                                        <button 
                                            className="btn btn-sm btn-outline-danger"
                                            onClick={() => removeFromCart(item.productId, item.machineId)}
                                        >
                                            &times;
                                        </button>
                                    </div>
                                </li>
                            ))}
                        </ul>
                    )}
                </div>

                <div className="border-top pt-3 mt-3">
                    <div className="d-flex justify-content-between mb-3">
                        <span className="h5">Suma:</span>
                        <span className="h5 text-primary">{cartTotal.toFixed(2)} PLN</span>
                    </div>
                    <button 
                        className="btn btn-success w-100 btn-lg" 
                        disabled={cartItems.length === 0}
                        onClick={onCheckout}
                    >
                        Zapłać teraz
                    </button>
                </div>
            </div>
        </>
    );
}

export default CartSidebar;