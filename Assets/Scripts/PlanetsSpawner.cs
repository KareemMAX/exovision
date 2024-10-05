using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
class PlanetData
{
    public Planet[] data;
}

[System.Serializable]
class Planet
{
    public string pl_name;
    public string sy_dist;
    public string glat;
    public string glon;
    public string pl_rade;
}

public class PlanetsSpawner : MonoBehaviour
{
    public float scale = 1000;

    // Start is called before the first frame update
    void Start()
    {
        var dataset = Resources.Load<TextAsset>("Data/planets");
        var planetsJson = JsonUtility.FromJson<PlanetData>(dataset.text);
        var planetsMap = planetsJson.data;
        
        foreach (var planet in planetsMap)
        {
            var planetPrefab = Resources.Load<GameObject>("Prefabs/Planet");
            // sy_dist is the distance in parsec units
            // glat is the galactic latitude
            // glon is the galactic longitude
            // Galaxy center is z+
            GameObject planetInstance;
            try
            {
                float glat = float.Parse(planet.glat);
                float glon = float.Parse(planet.glon);
                float sy_dist = float.Parse(planet.sy_dist) * scale;
                float x = sy_dist * Mathf.Cos(glat) * Mathf.Cos(glon);
                float y = sy_dist * Mathf.Cos(glat) * Mathf.Sin(glon);
                float z = sy_dist * Mathf.Sin(glat);
                planetInstance = Instantiate(planetPrefab, new Vector3(x, y, z), Quaternion.identity);
            }
            catch
            {
                continue;
            }
            // pl_rade is the radius in earth radii
            // Earth radius is 1000 units
            try
            {
                float pl_rade = float.Parse(planet.pl_rade) * 1000;
                planetInstance.transform.localScale = new Vector3(pl_rade, pl_rade, pl_rade);
            }
            catch (System.Exception)
            {
                planetInstance.transform.localScale = new Vector3(1000, 1000, 1000);
            }
            // Create a text object for the planet name
            var textPrefab = Resources.Load<Text>("Prefabs/NameText");
            var textInstance = Instantiate(textPrefab, planetInstance.transform.position, Quaternion.identity);
            planetInstance.GetComponent<NameTextBehaviour>().textHolder = textInstance;
            // Get canvas object
            var canvas = GameObject.Find("Canvas");
            textInstance.transform.SetParent(canvas.transform);
            textInstance.text = planet.pl_name;
            planetInstance.GetComponent<PlanetRotate>().rotateSpeed = Random.Range(-10, 10);

            // Get a random texture for the planet
            var textures = Resources.LoadAll<Texture>("Textures");
            var texture = textures[Random.Range(0, textures.Length)];
            planetInstance.GetComponent<Renderer>().material.mainTexture = texture;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
