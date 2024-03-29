using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// General class for generating mesh using marching cubes algorithm
/// reads data from int and float grids
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class MarchingCubes : MonoBehaviour
{
    private static readonly List<int[]> index = new List<int[]>() {
  new int[]{},
  new int[]{0, 8, 3},
  new int[]{0, 1, 9},
  new int[]{1, 8, 3, 9, 8, 1},
  new int[]{1, 2, 10},
  new int[]{0, 8, 3, 1, 2, 10},
  new int[]{9, 2, 10, 0, 2, 9},
  new int[]{2, 8, 3, 2, 10, 8, 10, 9, 8},
  new int[]{3, 11, 2},
  new int[]{0, 11, 2, 8, 11, 0},
  new int[]{1, 9, 0, 2, 3, 11},
  new int[]{1, 11, 2, 1, 9, 11, 9, 8, 11},
  new int[]{3, 10, 1, 11, 10, 3},
  new int[]{0, 10, 1, 0, 8, 10, 8, 11, 10},
  new int[]{3, 9, 0, 3, 11, 9, 11, 10, 9},
  new int[]{9, 8, 10, 10, 8, 11},
  new int[]{4, 7, 8},
  new int[]{4, 3, 0, 7, 3, 4},
  new int[]{0, 1, 9, 8, 4, 7},
  new int[]{4, 1, 9, 4, 7, 1, 7, 3, 1},
  new int[]{1, 2, 10, 8, 4, 7},
  new int[]{3, 4, 7, 3, 0, 4, 1, 2, 10},
  new int[]{9, 2, 10, 9, 0, 2, 8, 4, 7},
  new int[]{2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4},
  new int[]{8, 4, 7, 3, 11, 2},
  new int[]{11, 4, 7, 11, 2, 4, 2, 0, 4},
  new int[]{9, 0, 1, 8, 4, 7, 2, 3, 11},
  new int[]{4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1},
  new int[]{3, 10, 1, 3, 11, 10, 7, 8, 4},
  new int[]{1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4},
  new int[]{4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3},
  new int[]{4, 7, 11, 4, 11, 9, 9, 11, 10},
  new int[]{9, 5, 4},
  new int[]{9, 5, 4, 0, 8, 3},
  new int[]{0, 5, 4, 1, 5, 0},
  new int[]{8, 5, 4, 8, 3, 5, 3, 1, 5},
  new int[]{1, 2, 10, 9, 5, 4},
  new int[]{3, 0, 8, 1, 2, 10, 4, 9, 5},
  new int[]{5, 2, 10, 5, 4, 2, 4, 0, 2},
  new int[]{2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8},
  new int[]{9, 5, 4, 2, 3, 11},
  new int[]{0, 11, 2, 0, 8, 11, 4, 9, 5},
  new int[]{0, 5, 4, 0, 1, 5, 2, 3, 11},
  new int[]{2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5},
  new int[]{10, 3, 11, 10, 1, 3, 9, 5, 4},
  new int[]{4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10},
  new int[]{5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3},
  new int[]{5, 4, 8, 5, 8, 10, 10, 8, 11},
  new int[]{9, 7, 8, 5, 7, 9},
  new int[]{9, 3, 0, 9, 5, 3, 5, 7, 3},
  new int[]{0, 7, 8, 0, 1, 7, 1, 5, 7},
  new int[]{1, 5, 3, 3, 5, 7},
  new int[]{9, 7, 8, 9, 5, 7, 10, 1, 2},
  new int[]{10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3},
  new int[]{8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2},
  new int[]{2, 10, 5, 2, 5, 3, 3, 5, 7},
  new int[]{7, 9, 5, 7, 8, 9, 3, 11, 2},
  new int[]{9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11},
  new int[]{2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7},
  new int[]{11, 2, 1, 11, 1, 7, 7, 1, 5},
  new int[]{9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11},
  new int[]{5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0},
  new int[]{11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0},
  new int[]{11, 10, 5, 7, 11, 5},
  new int[]{10, 6, 5},
  new int[]{0, 8, 3, 5, 10, 6},
  new int[]{9, 0, 1, 5, 10, 6},
  new int[]{1, 8, 3, 1, 9, 8, 5, 10, 6},
  new int[]{1, 6, 5, 2, 6, 1},
  new int[]{1, 6, 5, 1, 2, 6, 3, 0, 8},
  new int[]{9, 6, 5, 9, 0, 6, 0, 2, 6},
  new int[]{5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8},
  new int[]{2, 3, 11, 10, 6, 5},
  new int[]{11, 0, 8, 11, 2, 0, 10, 6, 5},
  new int[]{0, 1, 9, 2, 3, 11, 5, 10, 6},
  new int[]{5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11},
  new int[]{6, 3, 11, 6, 5, 3, 5, 1, 3},
  new int[]{0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6},
  new int[]{3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9},
  new int[]{6, 5, 9, 6, 9, 11, 11, 9, 8},
  new int[]{5, 10, 6, 4, 7, 8},
  new int[]{4, 3, 0, 4, 7, 3, 6, 5, 10},
  new int[]{1, 9, 0, 5, 10, 6, 8, 4, 7},
  new int[]{10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4},
  new int[]{6, 1, 2, 6, 5, 1, 4, 7, 8},
  new int[]{1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7},
  new int[]{8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6},
  new int[]{7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9},
  new int[]{3, 11, 2, 7, 8, 4, 10, 6, 5},
  new int[]{5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11},
  new int[]{0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6},
  new int[]{9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6},
  new int[]{8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6},
  new int[]{5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11},
  new int[]{0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7},
  new int[]{6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9},
  new int[]{10, 4, 9, 6, 4, 10},
  new int[]{4, 10, 6, 4, 9, 10, 0, 8, 3},
  new int[]{10, 0, 1, 10, 6, 0, 6, 4, 0},
  new int[]{8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10},
  new int[]{1, 4, 9, 1, 2, 4, 2, 6, 4},
  new int[]{3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4},
  new int[]{0, 2, 4, 4, 2, 6},
  new int[]{8, 3, 2, 8, 2, 4, 4, 2, 6},
  new int[]{10, 4, 9, 10, 6, 4, 11, 2, 3},
  new int[]{0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6},
  new int[]{3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10},
  new int[]{6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1},
  new int[]{9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3},
  new int[]{8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1},
  new int[]{3, 11, 6, 3, 6, 0, 0, 6, 4},
  new int[]{6, 4, 8, 11, 6, 8},
  new int[]{7, 10, 6, 7, 8, 10, 8, 9, 10},
  new int[]{0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10},
  new int[]{10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0},
  new int[]{10, 6, 7, 10, 7, 1, 1, 7, 3},
  new int[]{1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7},
  new int[]{2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9},
  new int[]{7, 8, 0, 7, 0, 6, 6, 0, 2},
  new int[]{7, 3, 2, 6, 7, 2},
  new int[]{2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7},
  new int[]{2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7},
  new int[]{1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11},
  new int[]{11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1},
  new int[]{8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6},
  new int[]{0, 9, 1, 11, 6, 7},
  new int[]{7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0},
  new int[]{7, 11, 6},
  new int[]{7, 6, 11},
  new int[]{3, 0, 8, 11, 7, 6},
  new int[]{0, 1, 9, 11, 7, 6},
  new int[]{8, 1, 9, 8, 3, 1, 11, 7, 6},
  new int[]{10, 1, 2, 6, 11, 7},
  new int[]{1, 2, 10, 3, 0, 8, 6, 11, 7},
  new int[]{2, 9, 0, 2, 10, 9, 6, 11, 7},
  new int[]{6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8},
  new int[]{7, 2, 3, 6, 2, 7},
  new int[]{7, 0, 8, 7, 6, 0, 6, 2, 0},
  new int[]{2, 7, 6, 2, 3, 7, 0, 1, 9},
  new int[]{1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6},
  new int[]{10, 7, 6, 10, 1, 7, 1, 3, 7},
  new int[]{10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8},
  new int[]{0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7},
  new int[]{7, 6, 10, 7, 10, 8, 8, 10, 9},
  new int[]{6, 8, 4, 11, 8, 6},
  new int[]{3, 6, 11, 3, 0, 6, 0, 4, 6},
  new int[]{8, 6, 11, 8, 4, 6, 9, 0, 1},
  new int[]{9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6},
  new int[]{6, 8, 4, 6, 11, 8, 2, 10, 1},
  new int[]{1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6},
  new int[]{4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9},
  new int[]{10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3},
  new int[]{8, 2, 3, 8, 4, 2, 4, 6, 2},
  new int[]{0, 4, 2, 4, 6, 2},
  new int[]{1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8},
  new int[]{1, 9, 4, 1, 4, 2, 2, 4, 6},
  new int[]{8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1},
  new int[]{10, 1, 0, 10, 0, 6, 6, 0, 4},
  new int[]{4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3},
  new int[]{10, 9, 4, 6, 10, 4},
  new int[]{4, 9, 5, 7, 6, 11},
  new int[]{0, 8, 3, 4, 9, 5, 11, 7, 6},
  new int[]{5, 0, 1, 5, 4, 0, 7, 6, 11},
  new int[]{11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5},
  new int[]{9, 5, 4, 10, 1, 2, 7, 6, 11},
  new int[]{6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5},
  new int[]{7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2},
  new int[]{3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6},
  new int[]{7, 2, 3, 7, 6, 2, 5, 4, 9},
  new int[]{9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7},
  new int[]{3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0},
  new int[]{6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8},
  new int[]{9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7},
  new int[]{1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4},
  new int[]{4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10},
  new int[]{7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10},
  new int[]{6, 9, 5, 6, 11, 9, 11, 8, 9},
  new int[]{3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5},
  new int[]{0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11},
  new int[]{6, 11, 3, 6, 3, 5, 5, 3, 1},
  new int[]{1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6},
  new int[]{0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10},
  new int[]{11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5},
  new int[]{6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3},
  new int[]{5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2},
  new int[]{9, 5, 6, 9, 6, 0, 0, 6, 2},
  new int[]{1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8},
  new int[]{1, 5, 6, 2, 1, 6},
  new int[]{1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6},
  new int[]{10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0},
  new int[]{0, 3, 8, 5, 6, 10},
  new int[]{10, 5, 6},
  new int[]{11, 5, 10, 7, 5, 11},
  new int[]{11, 5, 10, 11, 7, 5, 8, 3, 0},
  new int[]{5, 11, 7, 5, 10, 11, 1, 9, 0},
  new int[]{10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1},
  new int[]{11, 1, 2, 11, 7, 1, 7, 5, 1},
  new int[]{0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11},
  new int[]{9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7},
  new int[]{7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2},
  new int[]{2, 5, 10, 2, 3, 5, 3, 7, 5},
  new int[]{8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5},
  new int[]{9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2},
  new int[]{9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2},
  new int[]{1, 3, 5, 3, 7, 5},
  new int[]{0, 8, 7, 0, 7, 1, 1, 7, 5},
  new int[]{9, 0, 3, 9, 3, 5, 5, 3, 7},
  new int[]{9, 8, 7, 5, 9, 7},
  new int[]{5, 8, 4, 5, 10, 8, 10, 11, 8},
  new int[]{5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0},
  new int[]{0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5},
  new int[]{10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4},
  new int[]{2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8},
  new int[]{0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11},
  new int[]{0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5},
  new int[]{9, 4, 5, 2, 11, 3},
  new int[]{2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4},
  new int[]{5, 10, 2, 5, 2, 4, 4, 2, 0},
  new int[]{3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9},
  new int[]{5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2},
  new int[]{8, 4, 5, 8, 5, 3, 3, 5, 1},
  new int[]{0, 4, 5, 1, 0, 5},
  new int[]{8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5},
  new int[]{9, 4, 5},
  new int[]{4, 11, 7, 4, 9, 11, 9, 10, 11},
  new int[]{0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11},
  new int[]{1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11},
  new int[]{3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4},
  new int[]{4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2},
  new int[]{9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3},
  new int[]{11, 7, 4, 11, 4, 2, 2, 4, 0},
  new int[]{11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4},
  new int[]{2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9},
  new int[]{9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7},
  new int[]{3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10},
  new int[]{1, 10, 2, 8, 7, 4},
  new int[]{4, 9, 1, 4, 1, 7, 7, 1, 3},
  new int[]{4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1},
  new int[]{4, 0, 3, 7, 4, 3},
  new int[]{4, 8, 7},
  new int[]{9, 10, 8, 10, 11, 8},
  new int[]{3, 0, 9, 3, 9, 11, 11, 9, 10},
  new int[]{0, 1, 10, 0, 10, 8, 8, 10, 11},
  new int[]{3, 1, 10, 11, 3, 10},
  new int[]{1, 2, 11, 1, 11, 9, 9, 11, 8},
  new int[]{3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9},
  new int[]{0, 2, 11, 8, 0, 11},
  new int[]{3, 2, 11},
  new int[]{2, 3, 8, 2, 8, 10, 10, 8, 9},
  new int[]{9, 10, 2, 0, 9, 2},
  new int[]{2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8},
  new int[]{1, 10, 2},
  new int[]{1, 3, 8, 9, 1, 8},
  new int[]{0, 9, 1},
  new int[]{0, 3, 8},
  new int[]{}
};

    //----------------------------
    //int grid functions
    //----------------------------
    private static Vector3 GetPos(int num, Vector3 offset)
    {
        Vector3 pos = Vector3.zero;
        if (num == 0) pos = new Vector3(0, -.5f, .5f);
        if (num == 1) pos = new Vector3(.5f, -.5f, 0);
        if (num == 2) pos = new Vector3(0, -.5f, -.5f);
        if (num == 3) pos = new Vector3(-.5f, -.5f, 0);
        if (num == 4) pos = new Vector3(0, .5f, .5f);
        if (num == 5) pos = new Vector3(.5f, .5f, 0);
        if (num == 6) pos = new Vector3(0, .5f, -.5f);
        if (num == 7) pos = new Vector3(-.5f, .5f, 0);
        if (num == 8) pos = new Vector3(-.5f, 0, .5f);
        if (num == 9) pos = new Vector3(.5f, 0, .5f);
        if (num == 10) pos = new Vector3(.5f, 0, -.5f);
        if (num == 11) pos = new Vector3(-.5f, 0, -.5f);

        pos += new Vector3(.5f, .5f, .5f);
        pos += offset;
        return pos;
    }
    /// <summary>
    /// Get case number based on a non empty vertices
    /// </summary>
    /// <param name="corners">Array of corners where 1 is full and 0 is empty</param>
    /// <returns></returns>
    private static int GetCase(int[] corners)
    {
        int res = 0;
        if (corners[0] == 0) res |= 1;
        if (corners[1] == 0) res |= 2;
        if (corners[2] == 0) res |= 4;
        if (corners[3] == 0) res |= 8;
        if (corners[4] == 0) res |= 16;
        if (corners[5] == 0) res |= 32;
        if (corners[6] == 0) res |= 64;
        if (corners[7] == 0) res |= 128;
        return res;
    }

    private static void MarchAlgorithm(int[,,] values, List<Vector3> vertices, List<int> faces)
    {
        Dictionary<Vector3, int> vertexInd = new Dictionary<Vector3, int>(); //FOr case with shared vertices
        for (int x = 0; x < values.GetLength(0) - 1; x++)
        {
            int nextX = x + 1 == values.GetLength(0) ? 0 : x + 1;
            for (int y = 0; y < values.GetLength(1) - 1; y++)
            {
                int nextY = y + 1 == values.GetLength(1) ? 0 : y + 1;
                for (int z = 0; z < values.GetLength(2) - 1; z++)
                {
                    int nextZ = z + 1 == values.GetLength(2) ? 0 : z + 1;

                    int[] corn = new int[] { values[x, y, nextZ], values[nextX, y, nextZ], values[nextX, y, z], values[x, y, z],
                        values[x, nextY, nextZ], values[nextX, nextY, nextZ], values[nextX, nextY, z], values[x, nextY, z] };

                    int currCase = GetCase(corn);
                    AddVertices(currCase, new Vector3(x, y, z), vertices, vertexInd);
                    AddTries(currCase, faces, new Vector3(x, y, z), vertexInd);

                }
            }
        }
    }

    public static Mesh GetMeshMarchingCubes(int[,,] values)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> faces = new List<int>();

        Mesh mesh = new Mesh();

        MarchAlgorithm(values, vertices, faces);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = faces.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }
    public static MeshData GetDataMarchingCubes(int[,,] values)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> faces = new List<int>();

        MarchAlgorithm(values, vertices, faces);

        return new MeshData(vertices, faces);
    }
    private static void AddTries(int ind, List<int> tries)
    {
        int triCount = index[ind].Length / 3;
        for (int i = 0; i < triCount; i++)
        {
            int startInd = tries.Count;
            tries.Add(startInd);
            tries.Add(startInd + 1);
            tries.Add(startInd + 2);
        }
    }
    /// <summary>
    /// Add indices with shared vertices
    /// </summary>
    /// <param name="ind"></param>
    /// <param name="tries"></param>
    /// <param name="offset"></param>
    /// <param name="vertexInd"></param>
    private static void AddTries(int ind, List<int> tries, Vector3 offset, Dictionary<Vector3, int> vertexInd)
    {
        for (int i = 0; i < index[ind].Length; i++)
        {
            Vector3 pos = GetPos(index[ind][i], offset);
            tries.Add(vertexInd[pos]);
        }
    }
    /// <summary>
    /// Add vertices making them shared
    /// </summary>
    /// <param name="ind"></param>
    /// <param name="offset"></param>
    /// <param name="vec"></param>
    /// <param name="vertexInd"></param>
    private static void AddVertices(int ind, Vector3 offset, List<Vector3> vec, Dictionary<Vector3, int> vertexInd)
    {
        for (int i = 0; i < index[ind].Length; i++)
        {
            Vector3 pos = GetPos(index[ind][i], offset);
            if (vertexInd.ContainsKey(pos))
            {
                continue;
            }
            vec.Add(pos);
            vertexInd.Add(pos, vec.Count - 1);
        }
    }
    //----------------------------
    //float grid functions
    //----------------------------
    /// <summary>
    /// General marching cubes algorithm
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="isoLevel">Threshold value</param>
    /// <param name="scaleValue">Value grid was upscaled with</param>
    /// <returns></returns>
    public static Mesh GetMeshMarchingCubes(float[,,] grid, float isoLevel, float scaleValue)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> faces = new List<int>();
        MarchAlgorithm(grid, vertices, faces, isoLevel, scaleValue);
        mesh.vertices = vertices.ToArray();
        mesh.triangles = faces.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }
    public static MeshData GetDataMarchingCubes(float[,,] grid, float isoLevel, float scaleValue)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> faces = new List<int>();
        MarchAlgorithm(grid, vertices, faces, isoLevel, scaleValue);
        MeshData data = new MeshData(vertices, faces);
        return data;
    }
    /// <summary>
    /// Algorithm with anonymous function to change scale based on position of the vertex
    /// </summary>
    /// <param name="values"></param>
    /// <param name="vertices"></param>
    /// <param name="faces"></param>
    /// <param name="isoLevel"></param>
    /// <param name="scaleValue"></param>
    public static void MarchAlgorithm(float[,,] values, List<Vector3> vertices, List<int> faces, float isoLevel, int heightThreshold, System.Func<int, int, int, Vector3> scaleValue)
    {
        for (int x = 0; x < values.GetLength(0) - 1; x++)
        {
            int nextX = x + 1 == values.GetLength(0) ? 0 : x + 1;
            for (int y = 0; y < values.GetLength(1) - 1; y++)
            {
                int nextY = y + 1 == values.GetLength(1) ? 0 : y + 1;
                for (int z = 0; z < values.GetLength(2) - 1; z++)
                {
                    int nextZ = z + 1 == values.GetLength(2) ? 0 : z + 1;

                    int[] corn = new int[] { Threshold(values[x, y, nextZ],isoLevel), Threshold(values[nextX, y, nextZ],isoLevel), Threshold(values[nextX, y, z],isoLevel), Threshold(values[x, y, z],isoLevel),
                        Threshold(values[x, nextY, nextZ],isoLevel), Threshold(values[nextX, nextY, nextZ],isoLevel), Threshold(values[nextX, nextY, z],isoLevel), Threshold(values[x, nextY, z],isoLevel) };

                    int currCase = GetCase(corn);
                    float[] cell = new float[] { values[x, y, nextZ], values[nextX, y, nextZ], values[nextX, y, z], values[x, y, z],
                    values[x, nextY, nextZ], values[nextX, nextY, nextZ], values[nextX, nextY, z], values[x, nextY, z]};
                    AddVertices(cell, currCase, new Vector3(x, y, z), isoLevel, vertices, scaleValue(x, y, z), heightThreshold);
                    //AddVertices(currCase, new Vector3(x, y, z), vertices, vertexInd);
                    AddTries(currCase, faces);

                }
            }
        }
    }
    /// <summary>
    /// Generates lists of vertices and indices from the passed grid
    /// </summary>
    /// <param name="values"></param>
    /// <param name="vertices"></param>
    /// <param name="faces"></param>
    /// <param name="isoLevel"></param>
    /// <param name="scaleValue"></param>
    private static void MarchAlgorithm(float[,,] values, List<Vector3> vertices, List<int> faces, float isoLevel, float scaleValue)
    {
        for (int x = 0; x < values.GetLength(0) - 1; x++)
        {
            int nextX = x + 1 == values.GetLength(0) ? 0 : x + 1;
            for (int y = 0; y < values.GetLength(1) - 1; y++)
            {
                int nextY = y + 1 == values.GetLength(1) ? 0 : y + 1;
                for (int z = 0; z < values.GetLength(2) - 1; z++)
                {
                    int nextZ = z + 1 == values.GetLength(2) ? 0 : z + 1;

                    int[] corn = new int[] { Threshold(values[x, y, nextZ],isoLevel), Threshold(values[nextX, y, nextZ],isoLevel), Threshold(values[nextX, y, z],isoLevel), Threshold(values[x, y, z],isoLevel),
                        Threshold(values[x, nextY, nextZ],isoLevel), Threshold(values[nextX, nextY, nextZ],isoLevel), Threshold(values[nextX, nextY, z],isoLevel), Threshold(values[x, nextY, z],isoLevel) };

                    int currCase = GetCase(corn);
                    float[] cell = new float[] { values[x, y, nextZ], values[nextX, y, nextZ], values[nextX, y, z], values[x, y, z],
                    values[x, nextY, nextZ], values[nextX, nextY, nextZ], values[nextX, nextY, z], values[x, nextY, z]};
                    AddVertices(cell, currCase, new Vector3(x, y, z), isoLevel, vertices, scaleValue);
                    AddTries(currCase, faces);

                }
            }
        }
    }
    private static void AddVertices(float[] vertices, int caseNum, Vector3 offset, float isoLevel, List<Vector3> vert, float scaleValue) => AddVertices(vertices, caseNum, offset, isoLevel, vert, new Vector3(scaleValue, 1, scaleValue));
    /// <summary>
    /// Adds vertices to array based on values in the grid
    /// </summary>
    /// <param name="vertices"></param>
    /// <param name="caseNum"></param>
    /// <param name="offset"></param>
    /// <param name="isoLevel"></param>
    /// <param name="vert"></param>
    /// <param name="scaleValue"></param>
    private static void AddVertices(float[] vertices, int caseNum, Vector3 offset, float isoLevel, List<Vector3> vert, Vector3 scaleValue, int heightThreshold = 0)
    {
        for (int i = 0; i < index[caseNum].Length; i++)
        {
            Vector3 pos = Vector3.zero;
            if (index[caseNum][i] == 0) pos = GetPos(new Vector3(0, 0, 1) + offset, new Vector3(1, 0, 1) + offset, vertices[0], vertices[1], isoLevel);
            else if (index[caseNum][i] == 1) pos = GetPos(new Vector3(1, 0, 1) + offset, new Vector3(1, 0, 0) + offset, vertices[1], vertices[2], isoLevel);
            else if (index[caseNum][i] == 2) pos = GetPos(new Vector3(1, 0, 0) + offset, new Vector3(0, 0, 0) + offset, vertices[2], vertices[3], isoLevel);
            else if (index[caseNum][i] == 3) pos = GetPos(new Vector3(0, 0, 0) + offset, new Vector3(0, 0, 1) + offset, vertices[3], vertices[0], isoLevel);
            else if (index[caseNum][i] == 4) pos = GetPos(new Vector3(0, 1, 1) + offset, new Vector3(1, 1, 1) + offset, vertices[4], vertices[5], isoLevel);
            else if (index[caseNum][i] == 5) pos = GetPos(new Vector3(1, 1, 1) + offset, new Vector3(1, 1, 0) + offset, vertices[5], vertices[6], isoLevel);
            else if (index[caseNum][i] == 6) pos = GetPos(new Vector3(1, 1, 0) + offset, new Vector3(0, 1, 0) + offset, vertices[6], vertices[7], isoLevel);
            else if (index[caseNum][i] == 7) pos = GetPos(new Vector3(0, 1, 0) + offset, new Vector3(0, 1, 1) + offset, vertices[7], vertices[4], isoLevel);
            else if (index[caseNum][i] == 8) pos = GetPos(new Vector3(0, 0, 1) + offset, new Vector3(0, 1, 1) + offset, vertices[0], vertices[4], isoLevel);
            else if (index[caseNum][i] == 9) pos = GetPos(new Vector3(1, 0, 1) + offset, new Vector3(1, 1, 1) + offset, vertices[1], vertices[5], isoLevel);
            else if (index[caseNum][i] == 10) pos = GetPos(new Vector3(1, 0, 0) + offset, new Vector3(1, 1, 0) + offset, vertices[2], vertices[6], isoLevel);
            else if (index[caseNum][i] == 11) pos = GetPos(new Vector3(0, 0, 0) + offset, new Vector3(0, 1, 0) + offset, vertices[3], vertices[7], isoLevel);
            pos.x /= scaleValue.x;
            pos.y /= scaleValue.y;
            pos.z /= scaleValue.z;
            if (heightThreshold != 0)
            {
                //pos += new Vector3(0, (scaleValue.y - 1) * 11 * scaleValue.y, 0);
                //pos += new Vector3(0, (scaleValue.y - 1) * heightThreshold / scaleValue.y, 0);
            }
            vert.Add(pos);
        }
    }
    /// <summary>
    /// Determines if point should be on or off
    /// </summary>
    /// <param name="val"></param>
    /// <param name="isoLevel"></param>
    /// <returns>Value 0 or 1</returns>
    private static int Threshold(float val, float isoLevel) => val > isoLevel ? 0 : 1;
    private static Vector3 GetPos(Vector3 p1, Vector3 p2, float v1, float v2, float isoLevel) => p1 + (isoLevel - v1) * (p2 - p1) / (v2 - v1);
}

public class MeshData
{
    public List<Vector3> vertices;
    public List<int> faces;

    public MeshData(List<Vector3> vertices, List<int> faces)
    {
        this.vertices = vertices;
        this.faces = faces;
    }

}