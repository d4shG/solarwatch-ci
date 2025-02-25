"use client"
import React, { useState, useEffect } from "react";
import Cookies from "js-cookie";

export default function SolarWatch() {
    const [city, setCity] = useState("");
    const [date, setDate] = useState("");
    const [sunTimes, setSunTimes] = useState(null);
    const [citySuggestions, setCitySuggestions] = useState([]);
    const [loading, setLoading] = useState(false);

    const handleSearch = async () => {
        const token = Cookies.get("AuthToken");

        if (!token) {
            console.error("No authentication token found.");
            return;
        }

        console.log(date)

        const response = await fetch(`api/solar-watch?city=${city}&date=${date}`, {
            method: 'GET',
            headers: {
            },
            credentials:'include'
        });

        if (response.ok) {
            const data = await response.json();
            const mockResponse = {
                sunrise: data.sunrise,
                sunset: data.sunset,
            };
            setSunTimes(mockResponse);
        } else {
            console.error('Solar data failed');
        }

    };

    const fetchCitySuggestions = async (query) => {
        setLoading(true);
        const token = Cookies.get("AuthToken");

        if (!token) {
            console.error("No authentication token found.");
            return;
        }
        const response = await fetch(`api/cities`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
            },
        });

        if (response.ok) {
            const data = await response.json();
            const filteredCities = data.filter(city =>
                city.toLowerCase().startsWith(query.toLowerCase())
            );
            setCitySuggestions(filteredCities);
        } else {
            console.error('Solar data failed');
        }       
        setLoading(false);
    };

    const handleSelectCity = (cityName) => {
        setCity(cityName)
    }

    useEffect(() => {
        if (city.length >= 3)
            fetchCitySuggestions(city)        
    }, [city])
    

    return (
        <main className="sunset-sunrise">
            <div className="top"></div>
            <div className="bottom"></div>
            <div className="content">
                <div>
                    <h1 className="header">
                        Sunrise & Sunset Times
                    </h1>
                    <form className="form-group">
                        <div className="form-group-cities">
                            <input
                                type="text"
                                id="city"
                                className="form-input"
                                placeholder="Type a city"
                                value={city}
                                onChange={(e) => setCity(e.target.value)}
                            />
                            {loading && <div>Loading...</div>}
                            {citySuggestions.length > 0 && (
                                <ul>
                                    {citySuggestions.map((cityName, index) => (
                                        <li
                                            key={index}
                                            onClick={() => handleSelectCity(cityName)}
                                            style={{ cursor: 'pointer' }}
                                        >
                                            {cityName}
                                        </li>
                                    ))}
                                </ul>
                            )}
                        </div>
                        <input
                            type="date"
                            id="date"
                            className="form-input"
                            value={date}
                            onChange={(e) => setDate(e.target.value)}
                        />
                        <button
                            type="button"
                            className="form-button"
                            onClick={handleSearch}
                        >
                            Get Times
                        </button>
                    </form>
                    {sunTimes && (
                        <div className="sunset-sunrise-results">
                            <h2 className="sunset-sunrise-results-header">Results:</h2>
                            <p className="sunset-sunrise-results-item">Sunrise: <span className="results-value sunrise">{sunTimes.sunrise}</span></p>
                            <p className="sunset-sunrise-results-item">Sunset: <span className="results-value sunset">{sunTimes.sunset}</span></p>
                        </div>
                    )}
                </div>
            </div>
        </main>
    );
}
