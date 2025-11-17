import { useState, useEffect } from 'react'
import { MapContainer, TileLayer, Marker, Popup } from 'react-leaflet'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'

const getIcon = (color) => {
  return new L.Icon({
    iconUrl: `https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-${color}.png`,
    shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png',
    iconSize: [25, 41],
    iconAnchor: [12, 41],
    popupAnchor: [1, -34],
    shadowSize: [41, 41]
  });
};

function App() {
  const [machines, setMachines] = useState([]);
  const [loading, setLoading] = useState(true);
  
  const [selectedMachine, setSelectedMachine] = useState(null);
  const [products, setProducts] = useState([]);
  const [productsLoading, setProductsLoading] = useState(false);
  const [notification, setNotification] = useState(null);

  const API_URL = import.meta.env.VITE_API_URL;
  const API_KEY = import.meta.env.VITE_API_KEY;

  // Pobieranie maszyn
  useEffect(() => {
    fetchMachines();
  }, []);

  const fetchMachines = () => {
    fetch(API_URL, {
      headers: { 'X-API-KEY': API_KEY }
    })
    .then(res => res.json())
    .then(data => {
      setMachines(data);
      setLoading(false);
    })
    .catch(err => console.error(err));
  };

  // Pobieranie produktów
  const openMachineModal = (machine) => {
    setSelectedMachine(machine);
    setProductsLoading(true);
    setProducts([]);

    fetch(`${API_URL}/${machine.id}/inventory`, {
      headers: { 'X-API-KEY': API_KEY }
    })
    .then(res => res.json())
    .then(data => {
      setProducts(data);
      setProductsLoading(false);
    });
  };

  // Kupowanie produktu
  const buyProduct = (productId) => {
    if(!confirm("Czy na pewno chcesz kupić ten produkt? (Symulacja płatności)")) return;

    fetch(`${API_URL}/${selectedMachine.id}/sale`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'X-API-KEY': API_KEY
      },
      body: JSON.stringify({ productId: productId })
    })
    .then(res => {
      if (res.ok) {
        showNotification("Sukces! Smacznego!", "success");
        openMachineModal(selectedMachine); 
      } else {
        res.json().then(err => showNotification("Błąd: " + err.message, "danger"));
      }
    })
    .catch(() => showNotification("Błąd połączenia z API", "danger"));
  };

  // Funkcja pomocnicza do powiadomień
  const showNotification = (msg, type) => {
    setNotification({ msg, type });
    setTimeout(() => setNotification(null), 3000);
  };


  if (loading) return <div className="text-center p-5">Ładowanie...</div>;

  return (
    <div className="container py-5 position-relative">
      
      {/* Powiadomienie Toast */}
      {notification && (
        <div className={`alert alert-${notification.type} position-fixed top-0 end-0 m-3`} style={{zIndex: 2000}}>
          {notification.msg}
        </div>
      )}

      {/* Nagłówek */}
      <div className="text-center mb-5">
        <h1 className="display-4 fw-bold text-primary">VendingManager</h1>
        <p className="lead text-muted">Wybierz automat i zrób zakupy</p>
      </div>

      {/* Mapa */}
      <div className="card shadow-sm border-0 mb-5 overflow-hidden">
        <div className="card-body p-0">
          <MapContainer center={[53.1325, 23.1688]} zoom={13} style={{ height: '400px', width: '100%' }}>
            <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
            {machines.map(m => {
               if (m.latitude === 0) return null;
               return (
                <Marker key={m.id} position={[m.latitude, m.longitude]} icon={getIcon(m.status === 'Online' ? 'green' : 'red')}>
                  <Popup>
                    <strong>{m.name}</strong><br/>
                    <button className="btn btn-sm btn-primary mt-2" onClick={() => openMachineModal(m)}>
                      Zakupy
                    </button>
                  </Popup>
                </Marker>
               )
            })}
          </MapContainer>
        </div>
      </div>

      {/* Lista Maszyn */}
      <div className="row g-4">
        {machines.map(machine => (
          <div key={machine.id} className="col-md-6 col-lg-4">
            <div className="card h-100 shadow-sm border-0">
              <div className="card-body">
                <h5 className="card-title fw-bold">{machine.name}</h5>
                <span className={`badge ${machine.status === 'Online' ? 'bg-success' : 'bg-danger'} mb-2`}>
                  {machine.status}
                </span>
                <p className="text-muted small">{machine.location}</p>
              </div>
              <div className="card-footer bg-transparent border-top-0">
                <button className="btn btn-outline-primary w-100" onClick={() => openMachineModal(machine)}>
                  Zobacz asortyment
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* === MODAL (Okienko Sklepu) === */}
      {selectedMachine && (
        <div className="modal show d-block" style={{backgroundColor: 'rgba(0,0,0,0.5)'}} tabIndex="-1">
          <div className="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">{selectedMachine.name}</h5>
                <button type="button" className="btn-close" onClick={() => setSelectedMachine(null)}></button>
              </div>
              <div className="modal-body">
                {productsLoading ? (
                  <div className="text-center py-5"><div className="spinner-border text-primary"></div></div>
                ) : (
                  <div className="row g-3">
                    {products.length === 0 && <p>Ten automat jest pusty!</p>}
                    
                    {products.map(p => (
                      <div key={p.productId} className="col-md-6">
                        <div className="card h-100">
                          <div className="row g-0 align-items-center">
                            <div className="col-4 p-2 text-center">
                                {p.imageUrl ? (
                                  <img src={p.imageUrl} className="img-fluid rounded" alt={p.productName} style={{maxHeight:'80px'}}/>
                                ) : (
                                  <div className="bg-light rounded d-flex align-items-center justify-content-center" style={{height:'80px'}}>Brak foto</div>
                                )}
                            </div>
                            <div className="col-8">
                              <div className="card-body p-2">
                                <h6 className="card-title mb-1">{p.productName}</h6>
                                <p className="card-text mb-1 fw-bold text-success">{p.price.toFixed(2)} PLN</p>
                                <p className="card-text small text-muted mb-2">Dostępne: {p.quantity} szt.</p>
                                <button 
                                  className="btn btn-sm btn-primary w-100" 
                                  disabled={p.quantity === 0}
                                  onClick={() => buyProduct(p.productId)}
                                >
                                  {p.quantity > 0 ? 'Kup teraz' : 'Wyprzedane'}
                                </button>
                              </div>
                            </div>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>
              <div className="modal-footer">
                <button type="button" className="btn btn-secondary" onClick={() => setSelectedMachine(null)}>Zamknij</button>
              </div>
            </div>
          </div>
        </div>
      )}

    </div>
  )
}

export default App