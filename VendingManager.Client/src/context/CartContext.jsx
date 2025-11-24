import { createContext, useState, useContext, useEffect } from 'react';

const CartContext = createContext();

export const useCart = () => useContext(CartContext);

export const CartProvider = ({ children }) => {
    const [cartItems, setCartItems] = useState(() => {
        try {
            const localData = localStorage.getItem('vendingCart');
            return localData ? JSON.parse(localData) : [];
        } catch (error) {
            console.error("Błąd odczytu localStorage:", error);
            return [];
        }
    });

    const [isCartOpen, setIsCartOpen] = useState(false);

    useEffect(() => {
        localStorage.setItem('vendingCart', JSON.stringify(cartItems));
    }, [cartItems]);

    const addToCart = (product, machineId) => {
        setCartItems(prevItems => {
            const existingItem = prevItems.find(item => item.productId === product.productId && item.machineId === machineId);

            if (existingItem && existingItem.count >= product.quantity) {
                return prevItems;
            }

            if (existingItem) {
                return prevItems.map(item => 
                    (item.productId === product.productId && item.machineId === machineId)
                    ? { ...item, count: item.count + 1 }
                    : item
                );
            }

            return [...prevItems, { 
                productId: product.productId,
                productName: product.productName || product.name,
                price: product.price,
                imageUrl: product.imageUrl,
                machineId, 
                count: 1 
            }];
        });
        setIsCartOpen(true);
    };

    const removeFromCart = (productId, machineId) => {
        setCartItems(prevItems => prevItems.filter(item => !(item.productId === productId && item.machineId === machineId)));
    };

    const decreaseQuantity = (productId, machineId) => {
        setCartItems(prevItems => {
            return prevItems.map(item => {
                if (item.productId === productId && item.machineId === machineId) {
                    return { ...item, count: item.count - 1 };
                }
                return item;
            }).filter(item => item.count > 0);
        });
    };

    const clearCart = () => setCartItems([]);

    const cartTotal = cartItems.reduce((total, item) => total + (item.price * item.count), 0);
    const cartCount = cartItems.reduce((count, item) => count + item.count, 0);

    return (
        <CartContext.Provider value={{ 
            cartItems, 
            addToCart, 
            removeFromCart,
            decreaseQuantity,
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