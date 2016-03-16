using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller.Common.Enumerations;
using SideScroller.Common.Interfaces;
using SideScroller.Common.HelperClasses;
using SideScroller.Common.Animations;
using SideScroller.Common.Animations.AnimationNames;
using SideScroller.Common.CollisionBoxes;

namespace SideScroller.Common.GameObjects.Weapons
{
    public abstract class Weapon : StandardGameObject, IDamaging
    {
        protected WeaponNames _name;
        protected WeaponType _type;
        protected int _damage;
        protected int _swingSpeed;
        protected Vector2 _weaponSize;

        protected Dictionary<CollisionBox, List<Vector2>> _attackLeftBoxOffsets;
        protected Dictionary<CollisionBox, List<Vector2>> _attackRightBoxOffsets;
        protected Dictionary<CollisionBox, List<Vector2>> _airAttackLeftBoxOffsets;
        protected Dictionary<CollisionBox, List<Vector2>> _airAttackRightBoxOffsets;

        protected Dictionary<CollisionBox, List<Vector2>> _attackDictionary;
        protected bool _attackFinished;

        public Weapon(RegionNames region, List<int> imageIndexes, Layer layer, Vector2 anchorPoint, int damage, int swingSpeed, Vector2 weaponSize)
            : base(region, imageIndexes, layer, anchorPoint, 0f)
        {
            _damage = damage;
            _swingSpeed = swingSpeed;

            _maskType = GameConstants.STANDARD_COLLISION_MASK;
            _mask = GameConstants.STANDARD_COLLISION_MASK + GameConstants.TERRAIN_COLLISION_MASK;

            _weaponSize = weaponSize;

            _attackLeftBoxOffsets = new Dictionary<CollisionBox, List<Vector2>>();
            _attackRightBoxOffsets = new Dictionary<CollisionBox, List<Vector2>>();
            _airAttackLeftBoxOffsets = new Dictionary<CollisionBox, List<Vector2>>();
            _airAttackRightBoxOffsets = new Dictionary<CollisionBox, List<Vector2>>();

            _attackFinished = true;
        }

        public void BeginAttack(bool aerial, Directions dir)
        {
            _attackFinished = false;

            if (aerial)
            {
                if (dir == Directions.Left)
                {
                    _animator.SetNewAnimation(WeaponAnimations.AERIAL_ATTACK_LEFT);
                    _attackDictionary = _attackLeftBoxOffsets;
                }
                else
                {
                    _animator.SetNewAnimation(WeaponAnimations.AERIAL_ATTACK_RIGHT);
                    _attackDictionary = _attackRightBoxOffsets;
                }
            }
            else
            {
                if (dir == Directions.Left)
                {
                    _animator.SetNewAnimation(WeaponAnimations.ATTACK_LEFT);
                    _attackDictionary = _airAttackLeftBoxOffsets;
                }
                else
                {
                    _animator.SetNewAnimation(WeaponAnimations.ATTACK_RIGHT);
                    _attackDictionary = _airAttackRightBoxOffsets;
                }
            }

            SetBoxOffsetBasedOnAnimationCounter();
        }

        public void ContinueAttack()
        {
            _animator.AdvanceAnimation();
            SetBoxOffsetBasedOnAnimationCounter();

            _attackFinished = _animator.AnimationFinished;
        }

        public void FinishAttack()
        {
            _attackFinished = true; // do this here so you can force stop attack if you need to
            _animator.SetNewAnimation(WeaponAnimations.HIDDEN);
        }

        protected void SetBoxOffsetBasedOnAnimationCounter()
        {
            foreach (var box in _attackDictionary.Keys)
            {
                box.AnchorPoint = _attackDictionary[box][_animator.AnimationCounter];
            }
        }

        public void UpdateAnchorPosition(Vector2 anchorObjectLocation)
        {
            _position = anchorObjectLocation; // + some offset
        }

        public WeaponNames Name
        {
            get { return _name; }
        }

        public WeaponType Type
        {
            get { return _type; }
        }

        public int Damage
        {
            get { return _damage; }
            set { _damage = value; }
        }

        public int SwingSpeed
        {
            get { return _swingSpeed; }
            set { _swingSpeed = value; }
        }

        public bool AttackFinished
        {
            get { return _attackFinished; }
        }
    }
}
