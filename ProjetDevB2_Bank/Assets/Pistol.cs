using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    [SerializeField] private int actualAmmoInLoader;
    [SerializeField] private int maxAmmoInLoader;
    [SerializeField] private int totalAmmo;

    [SerializeField] private float shootTime;
    [SerializeField] private float reloadTime;
    [SerializeField] private int damages;

    [SerializeField] private GameObject vfx;
    [SerializeField] private GameObject offset;

    public bool canShoot = true;
    private bool canReload;

    IEnumerator Shoot()
    {
        if (actualAmmoInLoader > 0)
        {
            canShoot = false;
            actualAmmoInLoader--;

            Instantiate(vfx, offset.transform.position, offset.transform.rotation);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.Log("Hit");
                if (hit.transform.CompareTag("NPC"))
                {
                    Debug.Log("NPC");
                    hit.transform.GetComponent<NPC>().TakeDamage(damages);
                }
            }


            yield return new WaitForSeconds(shootTime);
            canShoot = true;
        }
    }

    IEnumerator Reload()
    {
        canShoot = false;
        canReload = false;
        if (maxAmmoInLoader <= totalAmmo)
        {
            totalAmmo -= maxAmmoInLoader - actualAmmoInLoader;
            actualAmmoInLoader = maxAmmoInLoader;
        }
        else
        {
            if (actualAmmoInLoader + totalAmmo <= maxAmmoInLoader)
            {
                actualAmmoInLoader += totalAmmo;
                totalAmmo -= totalAmmo;
            }
            else
            {
                totalAmmo -= maxAmmoInLoader - actualAmmoInLoader;
                actualAmmoInLoader += maxAmmoInLoader - actualAmmoInLoader;
            }
        }
        yield return new WaitForSeconds(reloadTime);
        canShoot = true;
        canReload = true;
    }
}
