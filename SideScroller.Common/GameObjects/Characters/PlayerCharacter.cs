using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller.Common.Animations;
using SideScroller.Common.Animations.AnimationNames;
using SideScroller.Common.Enumerations;
using SideScroller.Common.CollisionBoxes;
using SideScroller.Common.HelperClasses;
using SideScroller.Common.Interfaces;
using SideScroller.Common.GameObjects.Weapons;

namespace SideScroller.Common.GameObjects.Characters
{
    public class PlayerCharacter : StandardGameObject, IDamageable
    {
        private readonly Vector2 SPRITE_SIZE = new Vector2(50, 100);

        private List<Weapon> _availableWeapons;
        private int _equippedWeapon;

        private CharacterStates _state;
        private Directions _facingDirection;

        private Vector2 _totalReboundDistance;

        private bool _okStand;

        private Vector2 _jumpedVelocity;
        private bool _ignoreDecay;
        private int _dashCounter;

        private bool _canContinueJump;
        private Vector2 _totalAdditionalJumpAccel;

        private CharacterStates _attackStartState;

        public PlayerCharacter(RegionNames region, List<int> imageIndexes, Layer layer, Vector2 anchorPoint, float gravitationalConstant)
            : base(region, imageIndexes, layer, anchorPoint, gravitationalConstant)
        {
            _mask = GameConstants.PHANTOM_COLLISION_MASK + GameConstants.TERRAIN_COLLISION_MASK + GameConstants.STANDARD_COLLISION_MASK;

            _animator = new Animator(ConstructAnimations(), PlayerCharacterAnimationNames.STAND_RIGHT);
            _state = CharacterStates.Stand;
            _collisionBoxes.Add(new RectangleCollisionBox(this, new Vector2(0, 0), (int)SPRITE_SIZE.X, (int)SPRITE_SIZE.Y, _mask));
            _availableWeapons = new List<Weapon>();
            _equippedWeapon = -1;

            _totalReboundDistance = new Vector2(0, 0);
            _facingDirection = Directions.Right;
            _jumpedVelocity = new Vector2(0, 0);
            _dashCounter = 0;

            _totalAdditionalJumpAccel = new Vector2(0, 0);

            _okStand = true;
        }

        #region Methods

        public override void UpdateVelocity()
        {
            var changedVelocity = _velocityPerFrame + _accelerationPerFrame;

            if (_state == CharacterStates.Jump) 
            {
                if (changedVelocity.Y > 0 && _velocityPerFrame.Y < 0)
                {
                    SetFallingAnimation();
                }
                else if (changedVelocity.Y < 0 && _velocityPerFrame.Y > 0)
                {
                    SetRisingAnimation();
                }
            }

            var maxX = GameConstants.MAX_RUN_VELOCITY_IN_X;

            if (_state == CharacterStates.Dash)
                maxX = GameConstants.DASH_VELOCITY;
            else if (_state == CharacterStates.Jump)
            {
                if (Math.Abs(_jumpedVelocity.X) > maxX)
                    maxX = Math.Abs(_jumpedVelocity.X);
            }

            if (Math.Abs(changedVelocity.X) <= maxX)
            {
                _velocityPerFrame.X = changedVelocity.X;
            }
            else
            {
                if (changedVelocity.X > 0)
                    _velocityPerFrame.X = maxX;
                else
                    _velocityPerFrame.X = -maxX;
            }

            if (Math.Abs(changedVelocity.Y) <= GameConstants.MAX_RUN_VELOCITY_IN_Y)
            {
                _velocityPerFrame.Y = changedVelocity.Y;
            }
            else
            {
                if (changedVelocity.Y > 0)
                    _velocityPerFrame.Y = GameConstants.MAX_RUN_VELOCITY_IN_Y;
                else
                    _velocityPerFrame.Y = GameConstants.MAX_RUN_VELOCITY_IN_Y;
            }

            _accelerationPerFrame = new Vector2(0, 0);
        }

        public override void DecayXVelocity(float multiplier)
        {
            if (_state == CharacterStates.Dash)
            {
                if (_dashCounter >= GameConstants.DASH_THRESHOLD)
                {
                    _state = CharacterStates.Walk;
                    SetWalkAnimation();
                }
                else
                {
                    _dashCounter++;
                }
                return;
            }

            if (!_ignoreDecay)
                base.DecayXVelocity(multiplier);
        }

        #region Collisions

        public override bool Collide(Vector2 currentObjectOffset, GameObject otherObject, Vector2 otherObjectOffset)
        {
            // need to handle collision if we hit an object
            // terrain objects need to rebound
            // movable terrain objects need to be pushed if possible
            // phantom objects need to edit the game indicator in the top right or something
            // enemeis need to begin to damage, or to flag that damage will happen next frame

            foreach (var collisionBox in _collisionBoxes)
            {
                foreach (var otherBox in otherObject.CollisionBoxes)
                {
                    var collided = collisionBox.Collide(currentObjectOffset, otherBox, otherObjectOffset);

                    if (collided)
                    {
                        this.HandleCollisionWithObject(currentObjectOffset, collisionBox, otherObject, otherObjectOffset, otherBox);
                        otherObject.HandleCollisionWithObject(otherObjectOffset, otherBox, otherObject, currentObjectOffset, collisionBox);
                    }
                }
            }

            return false;
        }

        public override void HandleCollisionWithObject(Vector2 currentObjectOffset, CollisionBox collidingBox, GameObject otherObject,
            Vector2 otherObjectOffset, CollisionBox otherCollidingBox)
        {
            _rebounded = false; // don't know if we need this here

            if (otherObject is TerrainObject)
            {
                var somethingNew = false;

                var directions = collidingBox.GetCollisionDirections(currentObjectOffset, otherCollidingBox, otherObjectOffset);
                var onTheFloor = false;

                var reboundDistance = otherCollidingBox.ReboundInShallowest(otherObjectOffset, collidingBox, currentObjectOffset);

                _totalReboundDistance += reboundDistance;
                _position += reboundDistance;
                _rebounded = true;

                if (directions.Contains(Directions.Up))
                {
                    _velocityPerFrame.Y = 0;
                }
                else if (directions.Contains(Directions.Down))
                {
                    _velocityPerFrame.Y = 0;
                    onTheFloor = true;

                    if (_state == CharacterStates.Jump)
                    {
                        somethingNew = true;
                        ExecuteLand();
                    }

                    if (_velocityPerFrame.X != 0)
                        DecayXVelocity(((TerrainObject)otherObject).Friction);
                    else
                        if (_state != CharacterStates.Recoil && _okStand) // this isn't going to work but just do this for now
                        {
                            // need to find some good way to get to stand without relying on velocity magnitude
                            _state = CharacterStates.Stand;
                            SetStandingAnimation();
                        }
                }

                if (directions.Contains(Directions.Left))
                {
                    _velocityPerFrame.X = 0;

                    if (onTheFloor)
                    {
                        if (_state == CharacterStates.Walk)
                        {
                            somethingNew = true;
                            BeginPush(Directions.Left);
                        }
                    }
                }
                else if (directions.Contains(Directions.Right))
                {
                    _velocityPerFrame.X = 0;

                    if (onTheFloor)
                    {
                        if (_state == CharacterStates.Walk)
                        {
                            somethingNew = true;
                            BeginPush(Directions.Right);
                        }
                    }
                }

                if (!somethingNew)
                {
                    ContinueCurrentExecution();
                }
            }
        }

        public void ResetReboundDistance()
        {
            _totalReboundDistance = new Vector2(0, 0);
        }

        #endregion

        #region ActionExecutions

        private void ContinueCurrentExecution()
        {
        }

        private void ExecuteLand()
        {
            UpdateFacingDirection();

            if (Math.Abs(_velocityPerFrame.X) > 0)
            {
                _state = CharacterStates.Walk;
                SetWalkAnimation();
            }
            else
            {
                _state = CharacterStates.Stand;
                SetStandingAnimation();
            }
        }

        public void ExecuteJump()
        {
            ApplyAccelerationVector(new Vector2(0, -5));
            _state = CharacterStates.Jump;

            if (Math.Abs(_velocityPerFrame.X) < 2)
            {
                if (_facingDirection == Directions.Left)
                    _jumpedVelocity = new Vector2(-2, _velocityPerFrame.Y);
                else
                    _jumpedVelocity = new Vector2(2, _velocityPerFrame.Y);
            }
            else
                _jumpedVelocity = _velocityPerFrame;

            _totalAdditionalJumpAccel = new Vector2(0, 0);
            _canContinueJump = true;
            SetRisingAnimation();
        }

        public void ContinueJump()
        {
            if (!_canContinueJump)
                return;

            if (_totalAdditionalJumpAccel.Y < -3)
            {
                _canContinueJump = false;
                return;
            }

            _totalAdditionalJumpAccel.Y = _totalAdditionalJumpAccel.Y - 0.5f;
            ApplyAccelerationVector(new Vector2(0, -0.5f));
        }

        public void CheckContinueJump(bool functionReady)
        {
            if (_state != CharacterStates.Jump || !functionReady)
                _canContinueJump = false;
        }

        public void ExecuteDash()
        {
            Console.WriteLine("DASH");
            _state = CharacterStates.Dash;
            _dashCounter = 0;

            if (_facingDirection == Directions.Left)
            {
                _velocityPerFrame.X = -GameConstants.DASH_VELOCITY;
                _animator.SetNewAnimation(PlayerCharacterAnimationNames.DASH_LEFT);
            }
            else
            {
                _velocityPerFrame.X = GameConstants.DASH_VELOCITY;
                _animator.SetNewAnimation(PlayerCharacterAnimationNames.DASH_RIGHT);
            }
        }

        public void StopDash()
        {
            _state = CharacterStates.Stand;
            SetStandingAnimation();
        }

        private void BeginPush(Directions dir)
        {
        }

        public void ExecuteMove(Directions dir) // pull out values into constants
        {
            _ignoreDecay = true;
            var additAccel = new Vector2(0, 0);

            if (_state == CharacterStates.Stand)
            {
                _state = CharacterStates.Walk;

                if (dir == Directions.Left)
                {
                    additAccel = new Vector2(-1, 0);
                    _animator.SetNewAnimation(PlayerCharacterAnimationNames.WALK_LEFT);
                }
                else if (dir == Directions.Right)
                {
                    additAccel = new Vector2(1, 0);
                    _animator.SetNewAnimation(PlayerCharacterAnimationNames.WALK_RIGHT);
                }
            }
            else if (_state == CharacterStates.Walk)
            {
                if (dir == Directions.Left)
                {
                    additAccel = new Vector2(-1, 0);

                    if (Math.Abs(_velocityPerFrame.X) > GameConstants.RUN_THRESHOLD_IN_X)
                        _animator.SetAnimationIfNotCurrent(PlayerCharacterAnimationNames.RUN_LEFT);
                }
                else if (dir == Directions.Right)
                {
                    additAccel = new Vector2(1, 0);

                    if (Math.Abs(_velocityPerFrame.X) > GameConstants.RUN_THRESHOLD_IN_X)
                        _animator.SetAnimationIfNotCurrent(PlayerCharacterAnimationNames.RUN_RIGHT);
                }
            }
            else if (_state == CharacterStates.Push)
            {
                if (dir == Directions.Left)
                {
                    if (_facingDirection != dir)
                    {
                        additAccel = new Vector2(-1, 0);
                        _animator.SetNewAnimation(PlayerCharacterAnimationNames.WALK_LEFT);
                    }
                    else
                    {
                        additAccel = new Vector2(-0.4f, 0);
                    }
                }
                else if (dir == Directions.Right)
                {
                    if (_facingDirection != dir)
                    {
                        additAccel = new Vector2(1, 0);
                        _animator.SetNewAnimation(PlayerCharacterAnimationNames.WALK_RIGHT);
                    }
                    else
                    {
                        additAccel = new Vector2(0.4f, 0);
                    }
                }
            }
            else // this means we are in the air
            { // need to set some max jump velocity here for when we jump 
                // can't go further than jump velocity in facing direction
                // the reverse direction should move slower

                if (_facingDirection == Directions.Left)
                {
                    if (dir == Directions.Left)
                    {
                        if (_velocityPerFrame.X + -1f < _jumpedVelocity.X)
                            _velocityPerFrame.X = _jumpedVelocity.X;
                        else
                            additAccel = new Vector2(-1, 0);
                    }
                    else if (dir == Directions.Right)
                    {
                        additAccel = new Vector2(0.5f, 0);
                    }
                    else if (dir == Directions.Down)
                    {
                        additAccel = new Vector2(0, 0.25f);
                    }
                }
                else
                {
                    if (dir == Directions.Left)
                    {
                        additAccel = new Vector2(-0.25f, 0);
                    }
                    else if (dir == Directions.Right)
                    {
                        if (_velocityPerFrame.X + 1f > _jumpedVelocity.X)
                            _velocityPerFrame.X = _jumpedVelocity.X;
                        else
                            additAccel = new Vector2(1, 0);
                    }
                    else if (dir == Directions.Down)
                    {
                        additAccel = new Vector2(0, 0.5f);
                    }
                }
            }

            if (additAccel.X != 0)
                _okStand = false;

            ApplyAccelerationVector(additAccel);
        }

        public void ExecuteAttack()
        {
            if (EquippedWeapon == null)
                return;

            _attackStartState = _state;
            _state = CharacterStates.Attack;

            if (_state == CharacterStates.Jump) // do an aerial attack
            {
                SetAttackAnimation(true);
            }
            else // do a ground attack
            {
                _velocityPerFrame.X = 0;
                SetAttackAnimation(false);
            }
        }

        public void CheckContinueAttack()
        {
            if (_state == CharacterStates.Attack)
            {
                if (EquippedWeapon.AttackFinished)
                {
                    SetStateToPreAttack();
                    EquippedWeapon.FinishAttack();
                    return;
                }

                EquippedWeapon.ContinueAttack();
            }
        }

        public void SetStateToPreAttack()
        {
            _state = _attackStartState;
        }

        #endregion

        #region Weapons

        public void AddWeaponToAvailable(Weapon weapon, int index)
        {
            if (index > 2 || index < 0)
                return;

            if (_availableWeapons.Count != 3)
            {
                _availableWeapons.Add(weapon);
                return;
            }

            _availableWeapons[index] = weapon;
        }

        public void SwitchWeapon()
        {
            _equippedWeapon++;

            if (_equippedWeapon > 2)
                _equippedWeapon = 0;

            // need to rebuild attack animations based on speed, this isn't entirely correct
            if (EquippedWeapon == null)
                return;

            var type = _availableWeapons[_equippedWeapon].Type;
            var speed = _availableWeapons[_equippedWeapon].SwingSpeed;

            if (type == WeaponType.Piercing)
            {
                _animator.Animations[PlayerCharacterAnimationNames.PIERCE_ATTACK_LEFT] = 
                    new Animation(PlayerCharacterAnimationNames.PIERCE_ATTACK_LEFT, 5, speed, SPRITE_SIZE);
                _animator.Animations[PlayerCharacterAnimationNames.PIERCE_ATTACK_RIGHT] =
                    new Animation(PlayerCharacterAnimationNames.PIERCE_ATTACK_RIGHT, 5, speed, SPRITE_SIZE);
                _animator.Animations[PlayerCharacterAnimationNames.PIERCE_ATTACK_AIR_LEFT] =
                    new Animation(PlayerCharacterAnimationNames.PIERCE_ATTACK_AIR_LEFT, 5, speed, SPRITE_SIZE);
                _animator.Animations[PlayerCharacterAnimationNames.PIERCE_ATTACK_AIR_RIGHT] =
                    new Animation(PlayerCharacterAnimationNames.PIERCE_ATTACK_AIR_RIGHT, 5, speed, SPRITE_SIZE);
            }
            else if (type == WeaponType.Blunting)
            {
                _animator.Animations[PlayerCharacterAnimationNames.BLUNT_ATTACK_LEFT] =
                    new Animation(PlayerCharacterAnimationNames.BLUNT_ATTACK_LEFT, 5, speed, SPRITE_SIZE);
                _animator.Animations[PlayerCharacterAnimationNames.BLUNT_ATTACK_RIGHT] =
                    new Animation(PlayerCharacterAnimationNames.BLUNT_ATTACK_RIGHT, 5, speed, SPRITE_SIZE);
                _animator.Animations[PlayerCharacterAnimationNames.BLUNT_ATTACK_AIR_LEFT] =
                    new Animation(PlayerCharacterAnimationNames.BLUNT_ATTACK_AIR_LEFT, 5, speed, SPRITE_SIZE);
                _animator.Animations[PlayerCharacterAnimationNames.BLUNT_ATTACK_AIR_RIGHT] =
                    new Animation(PlayerCharacterAnimationNames.BLUNT_ATTACK_AIR_RIGHT, 5, speed, SPRITE_SIZE);
            }
            else if (type == WeaponType.Slashing)
            {
                _animator.Animations[PlayerCharacterAnimationNames.SLASH_ATTACK_LEFT] =
                    new Animation(PlayerCharacterAnimationNames.SLASH_ATTACK_LEFT, 5, speed, SPRITE_SIZE);
                _animator.Animations[PlayerCharacterAnimationNames.SLASH_ATTACK_RIGHT] =
                    new Animation(PlayerCharacterAnimationNames.SLASH_ATTACK_RIGHT, 5, speed, SPRITE_SIZE);
                _animator.Animations[PlayerCharacterAnimationNames.SLASH_ATTACK_AIR_LEFT] =
                    new Animation(PlayerCharacterAnimationNames.SLASH_ATTACK_AIR_LEFT, 5, speed, SPRITE_SIZE);
                _animator.Animations[PlayerCharacterAnimationNames.SLASH_ATTACK_AIR_RIGHT] =
                    new Animation(PlayerCharacterAnimationNames.SLASH_ATTACK_AIR_RIGHT, 5, speed, SPRITE_SIZE);
            }

        }

        public void UpdateWeaponAnchorPositions()
        {
            foreach (var weap in _availableWeapons)
            {
                weap.UpdateAnchorPosition(this.Position);
            }
        }

        #endregion

        public void UpdateFacingDirection()
        {
            if (_state != CharacterStates.Walk && _state != CharacterStates.Push)
                return;

            Directions dir = _facingDirection;
            
            if (_accelerationPerFrame.X > 0)
            {
                dir = Directions.Right;
            }
            else if (_accelerationPerFrame.X < 0)
            {
                dir = Directions.Left;
            }

            if (dir == _facingDirection)
                return;

            _facingDirection = dir;

            if (dir == Directions.Left)
                _animator.SetNewAnimation(PlayerCharacterAnimationNames.WALK_LEFT);
            else if (dir == Directions.Right)
                _animator.SetNewAnimation(PlayerCharacterAnimationNames.WALK_RIGHT);
        }

        private void SetAttackAnimation(bool aerial)
        {
            if (EquippedWeapon == null)
                return;

            var weaponType = EquippedWeapon.Type;

            if (weaponType == WeaponType.Blunting)
            {
                if (aerial)
                {
                    if (_facingDirection == Directions.Left)
                        _animator.SetNewAnimation(PlayerCharacterAnimationNames.BLUNT_ATTACK_AIR_LEFT);
                    else
                        _animator.SetNewAnimation(PlayerCharacterAnimationNames.BLUNT_ATTACK_AIR_RIGHT);
                }
                else
                {
                    if (_facingDirection == Directions.Left)
                        _animator.SetNewAnimation(PlayerCharacterAnimationNames.BLUNT_ATTACK_LEFT);
                    else
                        _animator.SetNewAnimation(PlayerCharacterAnimationNames.BLUNT_ATTACK_RIGHT);
                }
            }
            else if (weaponType == WeaponType.Piercing)
            {
                if (aerial)
                {
                    if (_facingDirection == Directions.Left)
                        _animator.SetNewAnimation(PlayerCharacterAnimationNames.PIERCE_ATTACK_AIR_LEFT);
                    else
                        _animator.SetNewAnimation(PlayerCharacterAnimationNames.PIERCE_ATTACK_AIR_RIGHT);
                }
                else
                {
                    if (_facingDirection == Directions.Left)
                        _animator.SetNewAnimation(PlayerCharacterAnimationNames.PIERCE_ATTACK_LEFT);
                    else
                        _animator.SetNewAnimation(PlayerCharacterAnimationNames.PIERCE_ATTACK_RIGHT);
                }
            }
            else if (weaponType == WeaponType.Slashing)
            {
                if (aerial)
                {
                    if (_facingDirection == Directions.Left)
                        _animator.SetNewAnimation(PlayerCharacterAnimationNames.SLASH_ATTACK_AIR_LEFT);
                    else
                        _animator.SetNewAnimation(PlayerCharacterAnimationNames.SLASH_ATTACK_AIR_RIGHT);
                }
                else
                {
                    if (_facingDirection == Directions.Left)
                        _animator.SetNewAnimation(PlayerCharacterAnimationNames.SLASH_ATTACK_LEFT);
                    else
                        _animator.SetNewAnimation(PlayerCharacterAnimationNames.SLASH_ATTACK_RIGHT);
                }
            }
        }

        private void SetRisingAnimation()
        {
            if (_facingDirection == Directions.Left)
                _animator.SetNewAnimation(PlayerCharacterAnimationNames.RISING_LEFT);
            else
                _animator.SetNewAnimation(PlayerCharacterAnimationNames.RISING_RIGHT);
        }

        private void SetFallingAnimation()
        {
            if (_facingDirection == Directions.Left)
                _animator.SetNewAnimation(PlayerCharacterAnimationNames.FALLING_LEFT);
            else
                _animator.SetNewAnimation(PlayerCharacterAnimationNames.FALLING_RIGHT);
        }

        private void SetWalkAnimation()
        {
            if (_facingDirection == Directions.Left)
            {
                if (Math.Abs(_velocityPerFrame.X) > GameConstants.RUN_THRESHOLD_IN_X)
                    _animator.SetNewAnimation(PlayerCharacterAnimationNames.RUN_LEFT);
                else
                    _animator.SetNewAnimation(PlayerCharacterAnimationNames.WALK_LEFT);
            }
            else
            {
                if (Math.Abs(_velocityPerFrame.X) > GameConstants.RUN_THRESHOLD_IN_X)
                    _animator.SetNewAnimation(PlayerCharacterAnimationNames.RUN_RIGHT);
                else
                    _animator.SetNewAnimation(PlayerCharacterAnimationNames.WALK_RIGHT);
            }
        }

        private void SetStandingAnimation()
        {
            if (_facingDirection == Directions.Left)
                _animator.SetNewAnimation(PlayerCharacterAnimationNames.STAND_LEFT);
            else
                _animator.SetNewAnimation(PlayerCharacterAnimationNames.STAND_RIGHT);
        }

        private Dictionary<int, Animation> ConstructAnimations()
        {
            var animations = new Dictionary<int, Animation>();

            animations.Add(PlayerCharacterAnimationNames.STAND_LEFT,              new Animation(0,  1, 1, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.STAND_RIGHT,             new Animation(1,  1, 1, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.WALK_LEFT,               new Animation(2,  2, 3, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.WALK_RIGHT,              new Animation(3,  2, 3, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.RUN_LEFT,                new Animation(4,  2, 3, SPRITE_SIZE)); // might want to get rid of run
            animations.Add(PlayerCharacterAnimationNames.RUN_RIGHT,               new Animation(5,  2, 3, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.DASH_LEFT,               new Animation(6,  2, 3, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.DASH_RIGHT,              new Animation(7,  2, 3, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.JUMP_LEFT,               new Animation(8,  1, 1, SPRITE_SIZE)); // might want to get rid of jump too and just use rise
            animations.Add(PlayerCharacterAnimationNames.JUMP_RIGHT,              new Animation(9,  1, 1, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.RISING_LEFT,             new Animation(10, 1, 1, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.RISING_RIGHT,            new Animation(11, 1, 1, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.FALLING_LEFT,            new Animation(12, 1, 1, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.FALLING_RIGHT,           new Animation(13, 1, 1, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.RECOIL_LEFT,             new Animation(0,  1, 1, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.RECOIL_RIGHT,            new Animation(0,  1, 1, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.CAST_LEFT,               new Animation(0,  1, 1, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.CAST_RIGHT,              new Animation(0,  1, 1, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.SLASH_ATTACK_LEFT,       new Animation(14, 5, 3, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.SLASH_ATTACK_RIGHT,      new Animation(15, 5, 3, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.SLASH_ATTACK_AIR_LEFT,   new Animation(16, 5, 3, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.SLASH_ATTACK_AIR_RIGHT,  new Animation(17, 5, 3, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.PIERCE_ATTACK_LEFT,      new Animation(14, 5, 3, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.PIERCE_ATTACK_RIGHT,     new Animation(15, 5, 3, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.PIERCE_ATTACK_AIR_LEFT,  new Animation(16, 5, 3, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.PIERCE_ATTACK_AIR_RIGHT, new Animation(17, 5, 3, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.BLUNT_ATTACK_LEFT,       new Animation(14, 5, 3, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.BLUNT_ATTACK_RIGHT,      new Animation(15, 5, 3, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.BLUNT_ATTACK_AIR_LEFT,   new Animation(16, 5, 3, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.BLUNT_ATTACK_AIR_RIGHT,  new Animation(17, 5, 3, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.DIE_LEFT,                new Animation(0,  1, 1, SPRITE_SIZE));
            animations.Add(PlayerCharacterAnimationNames.DIE_RIGHT,               new Animation(0,  1, 1, SPRITE_SIZE));

            return animations;
        }

        public void ResetBools()
        {
            _ignoreDecay = false;
            _okStand = true;
        }

        #endregion

        public CharacterStates State
        {
            get { return _state; }
            set { _state = value; }
        }

        public Vector2 TotalReboundDistance
        {
            get { return _totalReboundDistance; }
        }

        public Directions FacingDirection
        {
            get { return _facingDirection; }
            set { _facingDirection = value; }
        }

        public List<Weapon> AvailableWeapons
        {
            get { return _availableWeapons; }
        }

        public Weapon EquippedWeapon
        {
            get 
            {
                if (_equippedWeapon != -1)
                    return _availableWeapons[_equippedWeapon];
                else
                    return null; // this might be dangerous
            }
        }
    }
}
