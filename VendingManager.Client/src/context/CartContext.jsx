import { createContext, useState, useContext } from 'react';

const CartContext = createContext();

export const useCart = () => useContext(CartContext);

export const CartProvider = ({ children }) => {
    const [cartItems, setCartItems] = useState([]);
    const [isCartOpen, setIsCartOpen] = useState(false);

    const addToCart = (product, machineId) => {
        setCartItems(prevItems => {
            const existingItem = prevItems.find(item => item.productId === product.productId && item.machineId === machineId);
            
            if (existingItem) {
                return prevItems.map(item => 
                    (item.productId === product.productId && item.machineId === machineId)
                    ? { ...item, count: item.count + 1 }
                    : item
                );
            }
            return [...prevItems, { ...product, machineId, count: 1 }];
        });
        setIsCartOpen(true);
    };

    const removeFromCart = (productId, machineId) => {
        setCartItems(prevItems => prevItems.filter(item => !(item.productId === productId && item.machineId === machineId)));
    };

    const clearCart = () => setCartItems([]);

    const cartTotal = cartItems.reduce((total, item) => total + (item.price * item.count), 0);
    const cartCount = cartItems.reduce((count, item) => count + item.count, 0);

    return (
        <CartContext.Provider value={{ 
            cartItems, 
            addToCart, 
            removeFromCart, 
            clearCart, 
            cartTotal, 
            cartCount,
            isCartOpen,
            setIsCartOpen
        }}>
            {children}
        </CartContext.Provider>
    );
};