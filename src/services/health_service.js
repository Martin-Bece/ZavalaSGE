import { obtener } from '@/services/ajax_service';

export function consultar_health(){
	return obtener('health');
}