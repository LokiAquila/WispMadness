extends Area2D


func _on_body_entered(body):
	modulate.a = 0.5
	body.modulate.a = 0.5
	


func _on_body_exited(body):
	modulate.a = 1
	body.modulate.a = 1
