﻿using UnityEngine;
using System.Collections;

public class PokemonObj : MonoBehaviour {
	public float speed = 5;
	public Vector3 velocity = Vector3.zero;
	public Pokemon pokemon = null;
	bool returning = false;

	void Update(){
		velocity -= rigidbody.velocity;
		velocity.y = 0;
		if (velocity.sqrMagnitude>speed*speed)	velocity = velocity.normalized*speed;
		rigidbody.AddForce(velocity, ForceMode.VelocityChange);
		velocity = Vector3.zero;

		if (pokemon!=null){
			foreach(Move move in pokemon.moves){
				move.cooldown += Time.deltaTime;
				move.cooldown = Mathf.Clamp01(move.cooldown);
			}

			if (pokemon.hp<=0){
				Return();
			}
		}
	}

	public void Return(){
		if (returning)	return;
		if (Player.pokemonObj==gameObject)	Player.pokemonActive = false;
		returning = true;
		GameObject effect = (GameObject)Instantiate(Resources.Load("ReturnEffect"));
		effect.transform.position = transform.position;
		effect.transform.parent = transform;
		Destroy(gameObject,1);
	}

	public bool UseMove(Vector3 direction, Move move){
		if (move.GetPPCost()>pokemon.pp)	return false;

		switch(move.moveType){

		case MoveNames.Growl:{
			if (move.cooldown<1)	return false;
			const float range = 10;
			foreach(GameObject enemyObj in GameObject.FindGameObjectsWithTag("pokemon")){
				if (enemyObj!=gameObject){
					if ((enemyObj.transform.position-transform.position).sqrMagnitude<range*range){
						GameObject newEffect = (GameObject)Instantiate(Resources.Load("Effects/Debuff"));
						newEffect.transform.position = enemyObj.transform.position+Vector3.up*0.2f;
						newEffect.transform.parent = enemyObj.transform;
					}
				}
			}
			audio.PlayOneShot((AudioClip)Resources.Load("Audio/Growl"));
			move.cooldown = 0;
			pokemon.pp-=move.GetPPCost();
			return true;}
			
		case MoveNames.TailWhip:{
			if (move.cooldown<1)	return false;
			const float range = 10;
			foreach(GameObject enemyObj in GameObject.FindGameObjectsWithTag("pokemon")){
				if (enemyObj!=gameObject){
					if ((enemyObj.transform.position-transform.position).sqrMagnitude<range*range){
						GameObject newEffect = (GameObject)Instantiate(Resources.Load("Effects/Debuff"));
						newEffect.transform.position = enemyObj.transform.position+Vector3.up*0.2f;
						newEffect.transform.parent = enemyObj.transform;
					}
				}
			}
			move.cooldown = 0;
			pokemon.pp-=move.GetPPCost();
			return true;}
			
		case MoveNames.Tackle:{
			if (move.cooldown<1)	return false;
			const float range = 2;
			RaycastHit[] hits = Physics.SphereCastAll(transform.position+Vector3.up, 1, direction ,range, 1<<10);
			foreach(RaycastHit hit in hits){
				if (hit.collider.gameObject!=gameObject){
					PokemonObj enemyObj = hit.collider.GetComponent<PokemonObj>();
					GameObject newEffect = (GameObject)Instantiate(Resources.Load("Effects/Bash"));
					newEffect.transform.position = hit.point;
					if (enemyObj){
						if (enemyObj.pokemon!=null)	enemyObj.pokemon.Damage(pokemon,move);
						if (GetComponent<PokemonPlayer>())	PokemonPlayer.target = enemyObj.gameObject;
						PokemonWild wildP = enemyObj.GetComponent<PokemonWild>();
						if (wildP)	wildP.enemy = gameObject;
					}
				}
			}
			rigidbody.AddForce(direction*range*rigidbody.mass*500);
			move.cooldown = 0;
			pokemon.pp-=move.GetPPCost();
			return true;}

		case MoveNames.Scratch:{
			if (move.cooldown<1)	return false;
			const float range = 2;
			RaycastHit[] hits = Physics.SphereCastAll(transform.position+Vector3.up, 1, direction ,range, 1<<10);
			foreach(RaycastHit hit in hits){
				if (hit.collider.gameObject!=gameObject){
					PokemonObj enemyObj = hit.collider.GetComponent<PokemonObj>();
					GameObject newEffect = (GameObject)Instantiate(Resources.Load("Effects/Scratch"));
					newEffect.transform.position = hit.point;
					if (enemyObj){
						if (enemyObj.pokemon!=null)	enemyObj.pokemon.Damage(pokemon,move);
						if (GetComponent<PokemonPlayer>())	PokemonPlayer.target = enemyObj.gameObject;
						PokemonWild wildP = enemyObj.GetComponent<PokemonWild>();
						if (wildP)	wildP.enemy = gameObject;
					}
					move.cooldown = 0;
					pokemon.pp-=move.GetPPCost();
					return true;
				}
			}
			GameObject neweffect = (GameObject)Instantiate(Resources.Load("Effects/Scratch"));
			neweffect.transform.position = transform.position+Vector3.up+direction;
			move.cooldown = 0;
			pokemon.pp-=move.GetPPCost();
			return true;}
		}

		return false;
	}
}