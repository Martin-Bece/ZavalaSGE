export const personas = [
	{
		id: 1,
		dni: '38123456',
		nombre: 'Martín',
		apellido: 'Zavala',
		telefono: '2664123456',
		direccion: 'Av. Illia 450, San Luis',
		rol: 'ADMIN'
	},
	{
		id: 2,
		nombre: 'Laura',
		apellido: 'Gómez',
		telefono: '2664654321',
		direccion: '', // Caso incompleto: persona sin dirección
		rol: 'PRECEPTOR'
	},
	{
		id: 3,
		dni: '29876543',
		nombre: 'Carlos',
		apellido: 'Rodríguez',
		telefono: '2664987654',
		direccion: 'San Martín 123, San Luis',
		rol: 'DOCENTE'
	}
];

export function obtener_personas() {
	return new Promise((resolve) => {
		setTimeout(() => {
			resolve(personas);
		}, 600);
	});
}