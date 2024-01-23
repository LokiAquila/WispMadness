extends Area2D


func _on_body_entered(body):
	if(body.name == "Player"):
		modulate.a = 0.5
		body.modulate.a = 0.5
	


func _on_body_exited(body):
	if(body.name == "Player"):
		modulate.a = 1
		body.modulate.a = 1
