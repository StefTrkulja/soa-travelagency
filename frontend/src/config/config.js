import { Icon, Style, Stroke } from 'ol/style';

export const defaultConfig = {
	baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api/gateway',
	mapZoom: 13.5,
	mapLocation: [19.823182951442245, 45.23942501835891],
	mapProjection: 'EPSG:3857',
	userAddressMarkerStyle: new Style({
		image: new Icon({
			anchor: [32, 35],
			anchorXUnits: 'pixels',
			anchorYUnits: 'pixels',
			scale: 0.65,
			src: 'https://maps.google.com/mapfiles/kml/shapes/ranger_station.png'
		})
	}),
	postLocationMarkerStyle: new Style({
		image: new Icon({
			anchor: [0.48, 0.75],
			scale: 0.15,
			src: '../icons/rabbit-marker.png'
		})
	}),
	clinicLocationMarkerStyle: new Style({
		image: new Icon({
			anchor: [0.48, 0.75],
			scale: 0.09,
			src: '../icons/vet-marker.png'
		})
	}),
	nearbyCircleStyle: new Style({
		stroke: new Stroke({
			color: 'black',
			width: 2,
			lineDash: [20, 25]
		}),
	}),
	nearbyCircleRadius: 4500, // NOTE: this is in meaters, but is bigger then actual size that backend uses to calculate nearby posts
};