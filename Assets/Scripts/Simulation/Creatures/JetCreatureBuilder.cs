using NeatJet.Scripts.Brain;
using NeatJet.Scripts.Simulation.Creatures.Genes;
using NeatJet.Scripts.Simulation.Creatures.Jets;

using UnityEngine;

namespace NeatJet.Scripts.Simulation.Creatures
{
    /// <summary>
    /// Contains functions to Create a <see cref="JetCreature"/> from a gene.
    /// </summary>
    public static class JetCreatureBuilder
    {
        /// <summary>
        /// Create a <see cref="JetCreature"/> from a <see cref="Gene"/>.
        /// </summary>
        /// <param name="gene">The gene from which to construct a creature.</param>
        /// <param name="name">The name to give the creature's parent object.</param>
        /// <param name="makeModel">Whether or not to make a visible model for the creature.</param>
        /// <returns>The GameObject containing the creature.</returns>
        public static JetCreature CreateCreature(Gene gene, string name, bool makeModel = true)
        {
            var creatureObject = new GameObject(name);
            var creature = JetCreature.Add(creatureObject, gene, makeModel);
            ColliderJet.AddMultiple(creatureObject, gene, makeModel);
            //StaticBrain.Add(creatureObject, gene);
            RandomBrain.Add(creatureObject, gene);
            return creature;
        }

        public static Gene DefaultCreatureGene { get; } = new Gene
            (0.4f,
            4,
            1f,
            0.2f,
            new(1f, 0.5f, 0f),
            2f,
            1f,
            20f,
            0.5f);
    }
}