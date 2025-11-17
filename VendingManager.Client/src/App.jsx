import { useState, useEffect } from 'react'
import { MapContainer, TileLayer, Marker, Popup } from 'react-leaflet'
import L from 'leaflet'

// --- KONFIGURACJA IKON ---
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
  const [error, setError] = useState(null);

  const API_URL = import.meta.env.VITE_API_URL;
  const API_KEY = import.meta.env.VITE_API_KEY;

  useEffect(() => {
    fetch(API_URL, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'X-API-KEY': API_KEY
      }
    })
    .then(response => {
      if (!response.ok) throw new Error('Błąd sieci lub zły klucz API');
      return response.json();
    })
    .then(data => {
      setMachines(data);
      setLoading(false);
    })
    .catch(err => {
      setError(err.message);
      setLoading(false);
    });
  }, []);

  if (loading) return (
    <div className="d-flex justify-content-center align-items-center vh-100">
      <div className="spinner-border text-primary" role="status">
        <span className="visually-hidden">Loading...</span>
      </div>
    </div>
  );

  if (error) return (
    <div className="container mt-5">
      <div className="alert alert-danger">Błąd: {error}</div>
    </div>
  );

  const mapCenter = [53.1325, 23.1688]; 

  return (
    <div className="container py-5">
      
      {/* Nagłówek */}
      <div className="text-center mb-5">
        <h1 className="display-4 fw-bold text-primary">VendingManager</h1>
        <p className="lead text-muted">Znajdź najbliższy automat w Twojej okolicy</p>
      </div>

      {/* === SEKCJA MAPY === */}
      <div className="card shadow-sm border-0 mb-5 overflow-hidden">
        <div className="card-body p-0">
          <MapContainer 
            center={mapCenter} 
            zoom={13} 
            style={{ height: '400px', width: '100%', zIndex: 0 }}
            scrollWheelZoom={false}
          >
            <TileLayer
              attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />
            
            {/* Generujemy markery dla maszyn */}
            {machines.map(machine => {
              if (machine.latitude === 0 && machine.longitude === 0) return null;

              const iconColor = machine.status === 'Online' ? 'green' : 'red';

              return (
                <Marker 
                  key={machine.id} 
                  position={[machine.latitude, machine.longitude]}
                  icon={getIcon(iconColor)}
                >
                  <Popup>
                    <strong>{machine.name}</strong><br />
                    {machine.location}<br/>
                    <span className={machine.status === 'Online' ? 'text-success' : 'text-danger'}>
                      ● {machine.status}
                    </span>
                  </Popup>
                </Marker>
              );
            })}
          </MapContainer>
        </div>
        <div className="card-footer bg-white text-center text-muted py-2">
          <small>Kliknij na pinezkę, aby zobaczyć szczegóły</small>
        </div>
      </div>

      {/* === LISTA MASZYN (KAFELKI) === */}
      <h3 className="mb-4 fw-bold border-bottom pb-2">Lista Automatów</h3>
      <div className="row g-4">
        {machines.map(machine => (
          <div key={machine.id} className="col-md-6 col-lg-4">
            <div className="card h-100 shadow-sm border-0 hover-effect">
              <div className="card-body">
                <div className="d-flex justify-content-between align-items-start mb-2">
                  <h5 className="card-title fw-bold">{machine.name}</h5>
                  <span className={`badge ${machine.status === 'Online' ? 'bg-success' : 'bg-danger'}`}>
                    {machine.status}
                  </span>
                </div>
                <h6 className="card-subtitle mb-3 text-muted">
                  {machine.location}
                </h6>
              </div>
              <div className="card-footer bg-transparent border-top-0">
                <button className="btn btn-outline-primary w-100 btn-sm">
                  Zobacz asortyment
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

export default App